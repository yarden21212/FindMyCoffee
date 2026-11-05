<h1 align="center">
  <img src="https://cdn-icons-png.flaticon.com/128/924/924514.png" width="60" alt="Coffee Cup" valign="middle">
  FindMyCoffee
  <img src="https://cdn-icons-png.flaticon.com/128/924/924514.png" width="60" alt="Coffee Cup" valign="middle">
</h1>
FindMyCoffee is a full-stack coffee-shop discovery application that helps people find cafés based on location and preferences. It also includes user accounts and a business workflow for adding coffee shops.

This was my first full-stack development project, built as a hands-on opportunity to deepen my understanding of frontend development, backend architecture, databases, and deployment.  

This repository contains the ASP.NET Core backend. The React frontend is maintained in a separate repository.  
Live application source: [Live Application](https://findmycoffee-app.netlify.app/)  

This repository contains the ASP.NET Core backend. The React frontend is maintained in a separate repository.
Frontend source: [Fronted](https://github.com-web-repo)
<h1></h1>

<h1>Features</h1>
<ul>
  <li><strong>Coffee-shop discovery:</strong> Search by name, type, given distance or even by the closest to my location, and explore results based on rating and distance.</li>
  <li><strong>Location-aware results:</strong> Calculate proximity from geographic coordinates using the Haversine formula.</li>
  <li><strong>Accounts and authentication:</strong> Register, log in, and log out using cookie-based authentication and hashed passwords.</li>
  <li><strong>Business registration:</strong> Allow registered users to become business users and create coffee-shop listings.</li>
  <li><strong>Address geocoding:</strong> Convert addresses into latitude/longitude using Geoapify-API.</li>
  <li><strong>Coffee-shop management:</strong> REST endpoints for reading, creating, updating, and deleting shop records.</li>
</ul>

<h1>Tech stack</h1>
<ul>
  <li><strong>Layer	Technologies.</li>
  <li><strong>Frontend (separate repository)	React, Vite, Tailwind CSS</li>
  <li><strong>Data access	Entity Framework Core, LINQ, AutoMapper</li>
  <li><strong>Database	PostgreSQL</li>
  <li><strong>Authentication	ASP.NET Core cookie authentication, BCrypt password hashing</li>
  <li><strong>Geocoding	Geoapify API</li>
  <li><strong>Deployment	Netlify (frontend), Railway (API and PostgreSQL)</li>
  <li><strong>API documentation	Swagger / OpenAPI</li>
</ul>



<h1>Architecture</h1>

<strong>FindMyCoffee follows a client-server architecture, separating the frontend presentation layer from the backend application logic.</strong>

Client: Browser → React + Vite frontend (Netlify)

API: React frontend → HTTP/JSON → ASP.NET Core Web API (Railway)

Database: ASP.NET Core → Entity Framework Core → PostgreSQL (Railway)

External service: ASP.NET Core → Geoapify API (Address Geocoding)

The frontend communicates with the backend through RESTful API endpoints.

The backend handles business logic, authentication, role-based authorization, request validation, and database operations using Entity Framework Core.

Coffee-shop information, user accounts, and their relationships are stored in PostgreSQL.

<h1>Screenshots & Videos</h1>
- Will be added soon - 
[Screenshots and a video walkthrough]

[Homepage and coffee-shop discovery]

[Search results and filtering]

[User registration and authentication]

[Business registration and coffee-shop creation]

<h1>Deployment</h1>

The application is publicly deployed using:

Frontend: Netlify

Backend: Railway

Database: PostgreSQL hosted on Railway

[Try FindMyCoffee Live](https://findmycoffee-app.netlify.app/)
