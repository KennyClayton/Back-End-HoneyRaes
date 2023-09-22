\c HoneyRaes

INSERT INTO Customer (Name, Address) VALUES ('Luke', '100 Main St.');
INSERT INTO Customer (Name, Address) VALUES ('Carroll', '1300 West Broad St');
INSERT INTO Customer (Name, Address) VALUES ('Bob', '97 Ravencrest Dr.');

INSERT INTO Employee (Name, Specialty) VALUES ('Gertrude', 'Plumbing');
INSERT INTO Employee (Name, Specialty) VALUES ('Tanner', 'Electrical');

INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency, DateCompleted) VALUES (1,1,'My kitchen sink drain line is busted.', true, '2023-09-20 15:30:00');
INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency, DateCompleted) VALUES (1,1,'The mower got the hose; the hose pulled the spigot off the wall.', false, '2023-09-20 15:20:00');
INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency, DateCompleted) VALUES (2,2,'The meter mast came down during the storm.', true, NULL);
INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency, DateCompleted) VALUES (2, NULL,'Lights were flickering after the storm', true);
INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency, DateCompleted) VALUES (3, NULL,'Toilet will not fill back up after flush.', true);
