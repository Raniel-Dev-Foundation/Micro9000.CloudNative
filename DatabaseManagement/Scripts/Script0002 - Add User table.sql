﻿CREATE TABLE Users
(
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Name VARCHAR(64) NOT NULL,
	IsDeleted BIT NOT NULL DEFAULT 0
)