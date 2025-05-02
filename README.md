# OOP
REPO  for OOP labs&amp;courseWork


ThirdLab - Student manager

Student Record Management

Student Operations: Implement the ability to add, edit, and view student records.

Input Validation: Ensure student data (name and grade) is validated.

In-memory storage: Store the records in JSON file (no database required).

API Integration: After adding a student, the system will fetch a motivational quote using the quotable.io API (or any other open, available API).

Display the quote in the console output.

Layered Architecture Implementation — Implement the system using a Layered Architecture with the following layers:

Application Layer: Manages the business logic and coordination between layers.

Domain Layer: Represents the core data and validation (Student entity and Quote validation).

Data Access Layer: Manages the persistence of data (for simplicity, an in-memory repository will be used).

DTO (Data Transfer Object) Implementation

StudentDTO: A DTO for transferring validated student data from the UI to the service.

QuoteDTO: A DTO for transferring the motivational quote fetched from the API.
