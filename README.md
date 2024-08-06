<h1>ChatAppAPI</h1>
<h3>Overview</h3> 
This project is an ASP.NET chat server with two branches: one using SQL for data storage and another using InMemory storage with hardcoded data to facilitate easy setup and testing. The chat server allows users to register, authenticate, and engage in conversations.

<h3>Branches</h3> 
<h5>SQL Branch</h5>
<ul>
  <li><strong>Purpose:</strong> Utilizes a SQL database for persistent storage of user and conversation data.<br></li>
  <li><strong>Setup:</strong> Requires configuring a SQL server and updating the connection string in the project configuration.<br></li>
</ul>

<h5>InMemory Branch</h5>
<ul>
  <li><strong>Purpose:</strong> Uses in-memory storage for easy setup and testing. Prepopulated with hardcoded data to allow the server to start with sample users and conversations.<br></li>
  <li><strong>Setup:</strong> No external database configuration needed. Ideal for quick testing and demonstration.<br></li>
</ul>

<h3>Features</h3>
<ul>
  <li><strong>User Registration and Authentication:</strong> Users can register and log in.</li>
  <li><strong>Conversations:</strong> Users can start conversations and send messages.</li>
  <li><strong>Data Storage:</strong> Supports both SQL and InMemory storage options.</li>
  <li><strong>API Documentation:</strong> Integrated with Swagger for easy API testing and interaction.</li>
</ul>

<h3>Project Structure</h3>
<ul>
  <li><strong>Controllers:</strong> Handle HTTP requests and responses.</li>
  <li><strong>Services:</strong> Contain business logic for user management and conversations.</li>
  <li><strong>Repositories:</strong> Abstract data access layer, interfacing with either SQL or InMemory data sources.</li>
  <li><strong>Models:</strong> Define the data structures used in the application.</li>
</ul>

<h3>Testing</h3>
<h5>SQL Branch</h5>
Ensure your SQL server is running and properly configured in appsettings.json before running tests.

<h5>InMemory Branch</h5>
Tests will use the hardcoded data, making it easy to verify the functionality without additional setup.

<h3>Contact</h3>
For any questions or issues, please contact <a href="mailto:mosmatan@gmail.com">mosmatan@gmail.com</a>.
