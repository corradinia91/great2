﻿--=========================================================================
-- Date: 13/08/2019
-- Description: fix EA refunds
-- Author: Marco Baccarani
--=========================================================================

UPDATE ExpenseAccount SET [IsRefunded] = 0 WHERE [Status] <> 2;

--=========================================================================
-- MANDATORY: Increment internal db version
PRAGMA user_version = 8;
--=========================================================================