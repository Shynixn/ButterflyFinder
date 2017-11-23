﻿CREATE TABLE IF NOT EXISTS SHY_TIMETASK
(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  filepath TEXT NOT NULL,
  timeschedule TEXT NOT NULL,
  lastindexedtime DATETIME,
  CONSTRAINT unique_filepath_cs UNIQUE (filepath),
  CONSTRAINT unique_timeschedule_cs UNIQUE (timeschedule)
);

CREATE TABLE IF NOT EXISTS SHY_FILECACHE
(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL,
  parent_id INTEGER,
  timetask_id INTEGER,
  CONSTRAINT forkey_parent_id_cs REFERENCES SHY_FILECACHES(id),
  CONSTRAINT forkey_timetask_id_cs UNIQUE REFERENCES SHY_TIMETASK(id)
)
