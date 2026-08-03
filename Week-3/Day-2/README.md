# Task Tracker API — SQL Server Schema Design & Normalization

## Overview

This document explains the database schema design for the Task Tracker API.

The goal of this design is to create a clean, normalized, and scalable database structure before implementing the API using ASP.NET Core and Entity Framework Core.

The design focuses on:

* Database normalization (1NF, 2NF, and 3NF)
* Primary keys and foreign keys
* Entity relationships
* Correct SQL Server data types
* ERD documentation

---

# 1. Entities and Attributes

The API resources require two main entities:

* Users
* Tasks

---

# Entity 1: Users

The Users entity represents the people who use the system and create tasks.

## Users Attributes

| Attribute | Description                            |
| --------- | -------------------------------------- |
| Id        | Unique identifier for the user         |
| Name      | User full name                         |
| Email     | User email address                     |
| CreatedAt | Date when the user account was created |

## Users Table

```
Users
----------------
Id
Name
Email
CreatedAt
```

---

# Entity 2: Tasks

The Tasks entity represents tasks created by users.

## Tasks Attributes

| Attribute   | Description                    |
| ----------- | ------------------------------ |
| Id          | Unique identifier for the task |
| Title       | Task title                     |
| Description | Task details                   |
| Status      | Current task status            |
| DueDate     | Task deadline                  |
| UserId      | User who owns the task         |
| CreatedAt   | Date when the task was created |

## Tasks Table

```
Tasks
----------------
Id
Title
Description
Status
DueDate
UserId
CreatedAt
```

---

# 2. Database Normalization

The schema was normalized using the three main normalization forms:

* First Normal Form (1NF)
* Second Normal Form (2NF)
* Third Normal Form (3NF)

Normalization helps prevent:

* Duplicate data
* Update anomalies
* Data inconsistency

---

# First Normal Form (1NF)

## Rule

A table is in 1NF when:

* Every column contains atomic values.
* No column contains multiple values.
* No repeating groups exist.

---

## Users Table - 1NF

```
Users
----------------
Id
Name
Email
CreatedAt
```

Analysis:

* Name contains one value.
* Email contains one value.
* CreatedAt contains one value.
* No comma-separated or repeated data exists.

Result:

✅ Users table satisfies First Normal Form (1NF).

---

## Tasks Table - 1NF

```
Tasks
----------------
Id
Title
Description
Status
DueDate
UserId
CreatedAt
```

Analysis:

* Each task has one title.
* Each task has one description.
* Each task has one status.
* Each task belongs to one user.

Result:

✅ Tasks table satisfies First Normal Form (1NF).

---

# Second Normal Form (2NF)

## Rule

A table is in 2NF when:

* It is already in 1NF.
* Every non-key attribute depends on the complete primary key.
* No partial dependency exists.

Partial dependency mainly happens when a table has a composite primary key.

---

## Users Table - 2NF

Primary Key:

```
Id
```

Dependencies:

```
Id → Name
Id → Email
Id → CreatedAt
```

The table has a single-column primary key, so there are no partial dependencies.

Result:

✅ Users table satisfies Second Normal Form (2NF).

---

## Tasks Table - 2NF

Primary Key:

```
Id
```

Dependencies:

```
Id → Title
Id → Description
Id → Status
Id → DueDate
Id → UserId
Id → CreatedAt
```

The table does not contain a composite key.

Result:

✅ Tasks table satisfies Second Normal Form (2NF).

---

# Third Normal Form (3NF)

## Rule

A table is in 3NF when:

* It is already in 2NF.
* Non-key attributes depend only on the primary key.
* There are no transitive dependencies.

---

## Incorrect Design Example

A bad Tasks table:

```
Tasks
----------------
Id
Title
UserId
UserName
UserEmail
```

Problem:

```
UserId → UserName
UserId → UserEmail
```

User information depends on UserId, not Task Id.

This creates a transitive dependency and violates 3NF.

---

## Correct 3NF Design

User information is stored only once in the Users table.

Users:

```
Users
----------------
Id PK
Name
Email
CreatedAt
```

Tasks:

```
Tasks
----------------
Id PK
Title
Description
Status
DueDate
UserId FK
CreatedAt
```

Result:

✅ Users table satisfies Third Normal Form (3NF).
✅ Tasks table satisfies Third Normal Form (3NF).

---

# 3. Primary Keys, Foreign Keys, and Relationships

## Primary Keys

### Users Table

```
Id INT PRIMARY KEY
```

Purpose:

* Uniquely identifies each user.
* Prevents duplicate users.

---

### Tasks Table

```
Id INT PRIMARY KEY
```

Purpose:

* Uniquely identifies each task.

---

# Foreign Key

Tasks table contains:

```
UserId INT FOREIGN KEY
```

Reference:

```
Tasks.UserId → Users.Id
```

Purpose:

* Ensures every task belongs to an existing user.
* Maintains referential integrity.

---

# Relationship

Relationship between Users and Tasks:

```
Users 1 -------- * Tasks
```

Explanation:

* One user can create many tasks.
* Each task belongs to one user.

Relationship Type:

```
One-to-Many
```

---

# 4. ERD Documentation

Two ERD diagrams were created using draw.io.

---

# ERD 1: Chen ER Diagram

Purpose:

The Chen ERD represents the conceptual database design.

It contains:

* Entities
* Attributes
* Relationships

Components:

* Rectangle → Entity
* Oval → Attribute
* Diamond → Relationship

Entities:

```
Users
Tasks
```

Relationship:

```
User Creates Tasks
```

---

# ERD 2: Crow's Foot ER Diagram

Purpose:

The Crow's Foot ERD represents the practical database structure used for implementation.

It shows:

* Tables
* Columns
* Primary Keys
* Foreign Keys
* Cardinality

Relationship:

```
Users 1 -------- * Tasks
```

Meaning:

One User can have many Tasks.

---

# 5. SQL Server Column Types

Choosing the correct data types improves storage efficiency and prevents invalid data.

---

# Users Table

| Column    | Data Type     | Reason                       |
| --------- | ------------- | ---------------------------- |
| Id        | INT IDENTITY  | Efficient primary key        |
| Name      | NVARCHAR(100) | Supports different languages |
| Email     | NVARCHAR(255) | Stores email values          |
| CreatedAt | DATETIME2     | Accurate date and time       |

---

# Tasks Table

| Column      | Data Type     | Reason                      |
| ----------- | ------------- | --------------------------- |
| Id          | INT IDENTITY  | Primary key                 |
| Title       | NVARCHAR(200) | Stores task titles          |
| Description | NVARCHAR(500) | Stores task details         |
| Status      | INT           | Stored as Enum value in C#  |
| DueDate     | DATETIME2     | Stores deadline information |
| UserId      | INT           | Foreign key reference       |
| CreatedAt   | DATETIME2     | Stores creation timestamp   |

---

# Final Database Schema

```
Users
--------------------------------
Id INT PRIMARY KEY
Name NVARCHAR(100)
Email NVARCHAR(255)
CreatedAt DATETIME2


Tasks
--------------------------------
Id INT PRIMARY KEY
Title NVARCHAR(200)
Description NVARCHAR(500)
Status INT
DueDate DATETIME2
UserId INT FOREIGN KEY
CreatedAt DATETIME2
```

---


# Conclusion

The Task Tracker API database design follows professional database practices:

✅ Entities identified correctly
✅ Schema normalized to 3NF
✅ Primary and Foreign Keys defined
✅ One-to-Many relationship implemented
✅ ERD diagrams documented
✅ SQL Server data types selected correctly
✅ Ready for EF Core migrations

This design provides a clean foundation for a scalable ASP.NET Core Web API.
