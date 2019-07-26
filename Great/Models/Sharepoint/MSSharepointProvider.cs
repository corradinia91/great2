﻿using AutoMapper;
using GalaSoft.MvvmLight.Messaging;
using Great.Models.Database;
using Great.Models.DTO;
using Great.Utils.Extensions;
using Great.Utils.Messages;
using Great.ViewModels.Database;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using static Great.Models.EventManager;

namespace Great.Models
{

    public class MSSharepointProvider
    {
        private ConcurrentQueue<EventEVM> eventQueue = new ConcurrentQueue<EventEVM>();
        public event EventHandler<EventChangedEventArgs> OnEventChanged;
        private Thread eventUpdaterThread;
        private Thread eventSenderThred;

        public MSSharepointProvider()
        {
            //Load all events
            using (DBArchive db = new DBArchive())
                eventQueue = new ConcurrentQueue<EventEVM>(db.Events.ToList().Select(e => new EventEVM(e)));


            eventUpdaterThread = new Thread(UpdaterThread);
            eventUpdaterThread.Name = "Sharepoint polling thread";
            eventUpdaterThread.IsBackground = true;
            eventUpdaterThread.Start();

            eventSenderThred = new Thread(SenderThread);
            eventSenderThred.Name = "Sharepoint sender thread";
            eventSenderThred.IsBackground = true;
            eventSenderThred.Start();
        }

        private void SenderThread()
        {
            bool exit = false;

            while (!exit)
            {
                while (eventQueue.Any(e => e.EStatus == EEventStatus.New || e.EStatus == EEventStatus.PendingCancel || e.EStatus == EEventStatus.PendingUpdate))
                {
                    EventEVM ev;
                    bool IsSent = false;

                    if (!eventQueue.TryDequeue(out ev))
                        continue;

                    do
                    {
                        try
                        {
                            XmlNode request = null;

                            switch (ev.EStatus)
                            {
                                case EEventStatus.New:

                                    request = GenerateBatchInsertXML(ev);
                                    using (SharepointReference.Lists l = new SharepointReference.Lists())
                                    {
                                        l.Credentials = new NetworkCredential(UserSettings.Email.Username, UserSettings.Email.EmailPassword);
                                        XmlDocument xdoc = new XmlDocument();

                                        var response = l.UpdateListItems("Vacations ITA", request.FirstChild);
                                        xdoc.LoadXml(response.OuterXml);
                                        var eventId = Convert.ToInt32(xdoc.GetElementsByTagName("z:row")[0]?.Attributes["ows_ID"].Value ?? "0");

                                        if (eventId > 0)
                                        {
                                            IsSent = true;
                                            ev.SharePointId = eventId;
                                            ev.EStatus = EEventStatus.Pending;
                                            ev.Save();
                                            eventQueue.Enqueue(ev);
                                            NotifyEventChanged(ev);
                                        }
                                    }
                                    break;

                                case EEventStatus.PendingUpdate:

                                    request = GenerateBatchUpdateXML(ev);
                                    using (SharepointReference.Lists l = new SharepointReference.Lists())
                                    {
                                        l.Credentials = new NetworkCredential(UserSettings.Email.Username, UserSettings.Email.EmailPassword);
                                        XmlDocument xdoc = new XmlDocument();

                                        var response = l.UpdateListItems("Vacations ITA", request.FirstChild);
                                        xdoc.LoadXml(response.OuterXml);
                                        var ecode = Convert.ToInt32(xdoc.GetElementsByTagName("ErrorCode")[0].Value);

                                        if (ecode == 0)
                                        {
                                            IsSent = true;
                                            ev.EStatus = EEventStatus.Pending;
                                            ev.Save();
                                            NotifyEventChanged(ev);
                                        }
                                    }
                                    break;

                                case EEventStatus.PendingCancel:

                                    request = GenerateBatchDeletetXML(ev);
                                    using (SharepointReference.Lists l = new SharepointReference.Lists())
                                    {
                                        l.Credentials = new NetworkCredential(UserSettings.Email.Username, UserSettings.Email.EmailPassword);
                                        XmlDocument xdoc = new XmlDocument();

                                        var response = l.UpdateListItems("Vacations ITA", request.FirstChild);
                                        xdoc.LoadXml(response.OuterXml);
                                        var ecode = Convert.ToInt32(xdoc.GetElementsByTagName("ErrorCode")[0].Value);

                                        if (ecode == 0)
                                        {
                                            IsSent = true;
                                            ev.EStatus = EEventStatus.Cancelled;
                                            ev.Save();
                                            NotifyEventChanged(ev);
                                        }
                                    }

                                    break;
                            }

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    while (!IsSent);
                }

                Thread.Sleep(ApplicationSettings.General.WaitForNextEmailCheck);
            }

        }

        private void UpdaterThread()
        {
            bool exit = false;

            while (!exit)
            {

                foreach (EventEVM e in eventQueue)
                {
                    if (e.EStatus != EEventStatus.Pending) continue;

                    var camlQueryString = string.Format("<Query><Where><Eq><FieldRef Name='ID' /><Value Type='Number'>{0}</Value></Eq></Where></Query>", e.SharePointId);
                    //var camlQueryString = string.Format("<Query><Where><Contains><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Contains></Where></Query>", e.SharePointId);

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(camlQueryString);
                    XmlNode n = doc.DocumentElement;

                    using (SharepointReference.Lists l = new SharepointReference.Lists())
                    {
                        l.Credentials = new NetworkCredential(UserSettings.Email.Username, UserSettings.Email.EmailPassword);
                        var response = l.GetListItems("Vacations ITA", null, n, null, null, null, null);

                        XmlDocument xdoc = new XmlDocument();
                        xdoc.LoadXml(response.OuterXml);
                        var newStatus = Convert.ToInt32(xdoc.GetElementsByTagName("z:row")[0]?.Attributes["ows__ModerationStatus"].Value ?? "1"); //default rejected
                        var approver = xdoc.GetElementsByTagName("z:row")[0]?.Attributes["ows_Editor"]?.Value;
                        var approvationdatetime = Convert.ToDateTime(xdoc.GetElementsByTagName("z:row")[0]?.Attributes["ows_Modified"].Value);

                        if (newStatus != e.Status)
                        {
                            e.Status = newStatus;
                            if (e.EStatus == EEventStatus.Accepted)
                            {
                                e.Approver = approver;
                                e.ApprovationDateTime = approvationdatetime;
                            }
                            e.Save();
                            NotifyEventChanged(e);
                        }
                    }
                }

                Thread.Sleep(ApplicationSettings.General.WaitForNextEmailCheck);
            }
        }
        protected void NotifyEventChanged(EventEVM e)
        {
            OnEventChanged?.Invoke(this, new EventChangedEventArgs(e));
        }
        public void Add(EventEVM e)
        {
            if (!eventQueue.Any(x => x.Id == e.Id))
                eventQueue.Enqueue(e);
        }
        public void Update(EventEVM e)
        {
            EventEVM existing = eventQueue.SingleOrDefault(x => x.Id == e.Id);

            if (existing == null) return;

            var id = existing.Id;
            Mapper.Map(e, existing);
            existing.EStatus = EEventStatus.PendingUpdate;
            existing.Id = id;
            existing.Save();

        }
        public void Delete(EventEVM e)
        {
            EventEVM existing = eventQueue.SingleOrDefault(x => x.Id == e.Id);

            if (existing == null) return;

            existing.EStatus = EEventStatus.PendingCancel;
            existing.Save();
        }
        private static XmlDocument GenerateBatchInsertXML(EventEVM ev)
        {
            var s = DateTime.Now.FromUnixTimestamp(ev.StartDateTimeStamp);
            var e = DateTime.Now.FromUnixTimestamp(ev.EndDateTimeStamp);

            var doc = new XDocument(
              new XElement("Batch",
              new XAttribute("OnError", "Continue"),
              new XAttribute("ListVersion", "1"),
              new XAttribute("ViewName", "Vacations ITA"),
              new XElement("Method",
                new XAttribute("ID", "1"),
                new XAttribute("Cmd", "New"),
                new XElement("Field", new XAttribute("Name", "Title"), ev.Description),
                new XElement("Field", new XAttribute("Name", "EventDate"), String.Format("{0:yyyy'-'MM'-'dd HH':'mm':'ss}", s)),
                new XElement("Field", new XAttribute("Name", "EndDate"), String.Format("{0:yyyy'-'MM'-'dd HH':'mm':'ss}", e)),
                new XElement("Field", new XAttribute("Name", "fAllDayEvent"), ev.IsAllDay ? 0 : 1),
                new XElement("Field", new XAttribute("Name", "EventType"), ev.Type - 1))));

            XmlDocument d = new XmlDocument();
            d.LoadXml(doc.ToString());
            return d;
        }
        private static XmlDocument GenerateBatchUpdateXML(EventEVM ev)
        {
            var s = DateTime.Now.FromUnixTimestamp(ev.StartDateTimeStamp);
            var e = DateTime.Now.FromUnixTimestamp(ev.EndDateTimeStamp);

            var doc = new XDocument(
              new XElement("Batch",
              new XAttribute("OnError", "Continue"),
              new XAttribute("ListVersion", "1"),
              new XAttribute("ViewName", "Vacations ITA"),
              new XElement("Method",
                new XAttribute("ID", "1"),
                new XAttribute("Cmd", "Update"),
                new XElement("Field", new XAttribute("Name", "ID"), ev.SharePointId),
                new XElement("Field", new XAttribute("Name", "Title"), ev.Description),
                new XElement("Field", new XAttribute("Name", "EventDate"), String.Format("{0:yyyy'-'MM'-'dd HH':'mm':'ss}", s)),
                new XElement("Field", new XAttribute("Name", "EndDate"), String.Format("{0:yyyy'-'MM'-'dd HH':'mm':'ss}", e)),
                new XElement("Field", new XAttribute("Name", "fAllDayEvent"), ev.IsAllDay ? 0 : 1),
                new XElement("Field", new XAttribute("Name", "EventType"), ev.Type - 1))));

            XmlDocument d = new XmlDocument();
            d.LoadXml(doc.ToString());
            return d;
        }
        private static XmlDocument GenerateBatchDeletetXML(EventEVM ev)
        {
            var doc = new XDocument(
            new XElement("Batch",
            new XAttribute("OnError", "Continue"),
                new XElement("Method",
                new XAttribute("ID", "1"),
                new XAttribute("Cmd", "Delete"),
                new XElement("Field", new XAttribute("Name", "ID"), ev.SharePointId))));

            XmlDocument d = new XmlDocument();
            d.LoadXml(doc.ToString());
            return d;
        }


    }

    public class EventChangedEventArgs : EventArgs
    {
        public EventEVM Ev { get; internal set; }

        public EventChangedEventArgs(EventEVM ev)
        {
            Ev = ev;
        }
    }
}