CREATE TABLE [dbo].[Tasks] (
    [TaskID]      INT           IDENTITY (1, 1) NOT NULL,
    [ProjectID]   INT           NULL,
    [TaskName]    VARCHAR (100) NOT NULL,
    [Description] TEXT          NULL,
    [Status]      VARCHAR (50)  DEFAULT ('Pending') NULL,
    [AssignedTo]  VARCHAR (8)   NULL,
    [AssignedBy] VARCHAR(4) NOT NULL, 
    PRIMARY KEY CLUSTERED ([TaskID] ASC),
    FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Projects] ([ProjectID]),
    FOREIGN KEY ([AssignedTo]) REFERENCES [dbo].[Students] ([RollNumber]),
    CHECK ([Status]='Completed' OR [Status]='In Progress' OR [Status]='Pending')
);

