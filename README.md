# lite-scrum
Lite Scrum is Agile and Scrum software productivity tool.
Youtube: What is lite scrum: https://youtu.be/KXNPRFq9D3w<br><br>
Lite Scrum saves you time doing your daily tasks. An example of a task would be :
"Go to grocery store" or "Add combo box for style preference selection for users on my site". 
Lite Scrum is not only for programmers, developers, and teams.It can be used by a single person,individually and also by a team.
<br>
<p>
  After you download the Zip File, and extract it, you will find:<br>
  <ul>
    <li>
      Folder WebApplication4 that contains ASP.NET MVC Visual Studio Solution to be hosted on IIS (Local or Remote or Cloud )
    </li>
    <li> File to create all the tables in SQL Database (CREATE_ALL_TABLES_SCRUM_DB)</li>
    <li> File to delete all the tables in SQL Database (DELETE_ALL_TABLES_SCRUM_DB)</li>
    <li>Infographic-Lite-Scrum.png</li>
<li>Scrum_Board.png</li>
<li>iPad_Lite_Scrum.png (Vertical view of LiteScrum on iPad)</li>
<li>iPad_Lite_Scrum_h (Horizontal view of LiteScrum on iPad)</li>

<li>Documentation.txt</li>
<li>TechnicalSpecs.txt (HOW DO WE USE LIGHT SCRUM)</li>
<li>Sources-Evolution of Agile.txt (links to evolution of agile throughout history)</li>
<li>Links.txt (useful links that explain the flow of lite scrum and use case)</li>
<li>what_light_scrum_can_do.txt (List of things that lite scrum can do)</li>
    </ul>
</p>
<p>
  <br>
  To run the application, you need first to:<br>
  <ol>
    <li>Create a database (SQL)</li>
    <li>Copy paste code of CREATE_ALL_TABLES_SCRUM_DB.sql file into Sql Server Management Studio. Please remember to change the database name to your own. This file uses SCRUM database name. You can create your own and change all the statements of USE [SCRUM] to USE [your own database name]</li>
    <li>Open solution in Visual Studio ( WebApplication4 folder ) or host it in IIS of your choice or cloud. But first change the web.config database connection to your own created above:</li>
    </ol>
  </p>
  
  <p>
  <br>
  This is the connectionstring in web.config that you need to change.The only things you need to change are:
  <ul>
  <li>
    YOURDATASOURCE (which is the address of your SQL server ex: LOCALDB\LocalUser or SQLServer112233.someserver.com) 
  </li>
    <li>
    YOURSQLDATABASE (The name of SQL database you created in previous step. Mine for example I called it SCRUM) 
  </li>
    <li>
    YOURSQLDATABASEUSERID (The user ID that you use to log in to your instance of SQL Server)
  </li>
    <li>
      YOURSQLDATABASEPASSWORD  (The password that you use to log in to your instance of SQL Server )
    </li>
  </ul>
  <br>
  ex:<br>

![Screenshot]([screenshot.png](https://github.com/jinan-kordab/lite-scrum/blob/master/Images/cs.png))
  </p>
  
  
