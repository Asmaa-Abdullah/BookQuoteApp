BookQuote

A responsive CRUD web application built with Angular 20 and .NET 9 Web API.
The application allows authenticated users to manage a shared collection of books and their own personal quotes.

Features
• User registration and login
• JWT authentication
• Protected API endpoints
• Books CRUD
• Personal quotes CRUD
• Favourite quotes
• "My Quotes" page showing up to 5 favourite quotes
• Responsive design for desktop, tablet, and mobile
• Bootstrap styling
• Font Awesome icons
• Light/Dark mode

Tech Stack
• Angular 20
• TypeScript
• .NET 9
• C#
• Entity Framework Core
• PostgreSQL
• JWT
• Bootstrap
• Font Awesome
• Docker / Docker Compose

Data

Books
Books are a shared collection. Authenticated users can view, add, edit, and delete books.
Quotes
Quotes belong to the authenticated user. Users can only view and manage their own quotes.

---

Running the Project

There are two ways to run the application.

1. Run Locally
   Use this option if you already have the required development tools installed.
   Requirements
   • .NET 9 SDK
   • Node.js
   • Angular CLI
   • PostgreSQL

Start the Backend

cd backend/BookQuoteApi
dotnet restore
dotnet run
The API runs on the configured local API URL.
Swagger is available in development mode.
Start the Frontend
Open another terminal:
cd frontend/bookquote-client
npm install
ng serve
The Angular application runs at:
http://localhost:4200
Make sure PostgreSQL is running and the database connection is configured correctly before starting the API.

---

2. Run with Docker Compose
   Use this option if you do not have .NET, Node.js, Angular CLI, or PostgreSQL installed.
   You only need:
   • Docker Desktop
   • Git
   Start the Application
   From the project root:
   cd infrastructure/docker
   docker compose up --build
   Docker Compose starts the complete application:
   • Angular frontend
   • .NET 9 API
   • PostgreSQL database
   No local PostgreSQL installation is required.
   Open the Application
   Frontend:
   http://localhost:4200
   API:
   http://localhost:5118
   Swagger:
   http://localhost:5118/swagger
   Stop the Application
   docker compose down

---

Project Structure
BookQuoteApp/
├── backend/
│ ├── BookQuoteApi/
│ └── BookQuoteApi.Tests/
│
├── frontend/
│ └── bookquote-client/
│
├── infrastructure/
│ └── docker/
│ └── docker-compose.yml
│
├── .gitignore
└── README.md

Testing
Run backend tests:
cd backend
dotnet test
Build the Angular application:
cd frontend/bookquote-client
ng build
Live Demo
Application: <LIVE_APP_URL>
