CREATE TABLE IF NOT EXISTS SHY_TIMETASK
(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  filepath TEXT NOT NULL,
  timeschedule TEXT NOT NULL,
  lastindexedtime DATETIME,
  CONSTRAINT unique_filepath_cs UNIQUE (filepath)
);