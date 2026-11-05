<h1 align="center">
  <img src="https://cdn-icons-png.flaticon.com/128/924/924514.png" width="60" alt="Coffee Cup" valign="middle">
  FindMyCoffee
  <img src="https://cdn-icons-png.flaticon.com/128/924/924514.png" width="60" alt="Coffee Cup" valign="middle">
</h1>
FindMyCoffee is a full-stack coffee-shop discovery application that helps people find cafés based on location and preferences. It also includes user accounts and a business workflow for adding coffee shops.
This project was my first full-stack project, I tried to avoid AI in order to learn coding, so if the design doesn't look perfect and if some functionality doesn't work perfectly, then that's the reason!

<h1 align="center">The next project will be <strong >PERFECTO</strong>!</h1>

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
                  User's browser
                        |
                        v
             React + Vite frontend
                  (Netlify)
                        |
                    HTTP / JSON
                        |
                        v
              ASP.NET Core Web API
                    (Railway)
                  /           \
                 v             v
        Entity Framework     Geoapify API
             Core            (geocoding)
                 |
                 v
         PostgreSQL database
               (Railway)
The frontend communicates with the backend through REST endpoints. The API handles application logic, authentication, validation, database access, and geocoding. PostgreSQL stores user and coffee-shop information.
Screenshots and demo
Screenshots and a video walkthrough will be added here.

Suggested media to include:
- Homepage and coffee-shop search
- Search results and filters
- Login and registration
- Business registration and adding a coffee shop
<!--
When you have screenshots, add them to docs/images/ and insert e.g.:
![Search results](docs/images/search-results.png)

For large videos, link to a hosted walkthrough rather than committing a large
video file to the Git repository.
-->API examples
The deployed API base URL is:
https://findmycoffee-production.up.railway.app
Method	Endpoint	Purpose
GET	/api/CoffeeShop/GetShops	Retrieve coffee shops
POST	/api/CoffeeShop/FindClosestCoffeeshops	Find nearby shops
POST	/api/CoffeeShop/FindByRating	Search by rating
POST	/api/CoffeeShop/GetShopsByName	Search by name
POST	/api/CoffeeShop/GetShopsByType	Search by type
POST	/api/auth/login	Log in
POST	/api/auth/logout	Log out
POST	/api/Business/RegisterAsBusiness	Register as a business
POST	/api/CoffeeShop/CreateShop	Create a coffee shop (business role)


Some endpoints require JSON request bodies or an authenticated session. See the controller actions and Swagger documentation for request/response details.

Run the backend locally
Prerequisites
- .NET 9 SDK
- PostgreSQL
- A Geoapify API key for geocoding
1. Clone and restore dependencies
git clone https://github.com/yarden21212/FindMyCoffee.git
cd FindMyCoffee
dotnet restore FindMyCoffee.sln
2. Configure local secrets
The repository does not store API keys or database passwords. Use ASP.NET Core User Secrets during development.

From the repository root:
dotnet user-secrets set "ConnectionStrings:CoffeeDbConnection" "Host=localhost;Port=5432;Database=findmycoffee;Username=postgres;Password=YOUR_LOCAL_PASSWORD" --project FindMyCoffee/FindMyCoffee.csproj

dotnet user-secrets set "Settings:GeoapifyApiKey" "YOUR_GEOAPIFY_API_KEY" --project FindMyCoffee/FindMyCoffee.csproj
Replace the placeholders with your own development credentials. Never commit secrets to Git.
3. Apply database migrations
If the EF Core CLI tool is installed:
dotnet ef database update --project FindMyCoffee/FindMyCoffee.csproj
4. Start the API
dotnet run --project FindMyCoffee/FindMyCoffee.csproj
Use the URL printed in the terminal. Swagger UI is available at /swagger when enabled.
Deployment
- Frontend: Netlify — https://findmycoffee-app.netlify.app/
- Backend: Railway — https://findmycoffee-production.up.railway.app
- Database: PostgreSQL on Railway
Production credentials are provided through Railway environment variables, including:
ConnectionStrings__CoffeeDbConnection
Settings__GeoapifyApiKey
ASP.NET Core maps __ to : in configuration keys. The backend's root / route may return 404; use an API route such as /api/CoffeeShop/GetShops to test it.
Backend source: FindMyCoffee · Live app: findmycoffee-app.netlify.app
