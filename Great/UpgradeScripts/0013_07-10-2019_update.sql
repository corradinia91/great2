﻿--=========================================================================
-- Date: 07/10/2019
-- Description: events bugfix
-- Author: Marco Baccarani
--=========================================================================

DELETE FROM Event;
DELETE FROM Day WHERE strftime('%H:%M:%S', datetime(timestamp, 'unixepoch')) <> '00:00:00';

--=========================================================================
-- MANDATORY: Increment internal db version
PRAGMA user_version = 13;
--=========================================================================