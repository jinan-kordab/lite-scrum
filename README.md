# lite-scrum
Lite Scrum is Agile and Scrum software productivity tool.
Youtube: What is lite scrum: https://youtu.be/KXNPRFq9D3w<br><br>

<p>
  <h5>Things Light Scrum Does</h5>
<ul>
  
<li>Keep track of things you want to do</li>
<li>Keep track of the things you are doing right now</li>
<li>Keep track of the things you are finished doing</li>
<li>Move same things around, from to do , to doing , to done</li>
<li>Transfer tasks to Quality Assurance person</li>
<li>Adhere to basic Scrum ideologies</li>
<li>Keeps things open and transparent</li>
<li>Helps achieve Hyperproductivity</li>
<li>View all your team’s progress in Scrum Board</li>
<li>Allow all the members to create individual items from particular product backlog ma assign them to whoever is in the same team.</li>
<li>Allow team members to create individual tasks and assign them from within their personal home scrum board.</li>
<li>Gives a user a choice to move his or her task item to QA or skip QA and move it directly to DONE. This is helpful for a single person team, or for teams who do not have QA personnel. Ex: Geocery store buying, Wedding planner .</li>
<li>Add or create, delete, and edit new Clients</li>
<li>Add or create, delete, and edit new Epics</li>
<li>Add or create, delete, and edit new Product Backlogs</li>
<li>Add or create, delete, and edit new Individual Tasks</li>
  </ul>
</p>




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

<p align="center">
  <img src="/Images/cs.png" alt="webconfig"/>
</p>
<br>

Also, in the same WebConfig, you need to change your SMTP settings, if you want to keep the SMTP functionality. 
It is mainly used to send email to reset password and registration confirmation.<br>
<br>
So to change the SMTP server of your choice, in the same web.config please find the appSettings section:<br>
<img src="/Images/as.png" alt="smtpsettings"/>
  </p>
  <p>
  <br>
  After above steps are performed, start your project or navigate to it. 
  </p>
  
<p>
  
  <h5><u>HOW DO WE USE LIGHT SCRUM</u></h5>
<br>

<b>1-Where do I start ?</b>
<ul>
	<li>Register, in order to have an account, then log in using your registration nickname and password.</li>
  <li>On Scrum Board landing page, you will see four empty columns, TODO, INPROGRESS, QA, DONE</li>
  <li>The first thing would be to create your first TODO task item.
		For that click on three vertical dots near TODO column 	 title, and click on New Item.</li>
	<li>A window apears, and the first three "must" options to select are Client, Epic, and Product Backlog. If you have not 	 created each one of them before, go ahead and create them now, in the following sequence, Client, Epic, Product 	 Backlog.</li>


<li>
This is needed to link each new task item you create to a particular client. Now since you can and will have many clients, each client can have one or many epics, and in turn each epic, can or will have one or many Product 	 	 Backlogs, and finally, each Product Backlog can have one or many Task Items, which you see on the landing page.This 	 	 connection will later ve visible when you navigate to Sprint. This is done from one side to lessen the pressure on you 	 from specifying time frame for each of your items and sprints that you create, and directly jump to work, thus increasing Hyperproductivity.
  </li>
  </ul>
  <br>
  
<b>2-Creating a Client</b>
<ul>
  <li>
	To create a new Client, click on Clients menu at the top navigation bar. Then, next to the Clients header title, there are three vertical dots. Click on them, and you will see the option New Client. Fill out the window that comes up, and save changes.
  </li>
  </ul>
<br>

<b>3-Creating an Epic</b>
<ul>
  <li>
	In order to create an Epic, we need to be on the selected Epics page that relate to particular Client.To create a new Epic, first navigate to all epics page by clicking on three vertical dots inside selected client on Clients page. You will see the menu pops up with the following options: All Epics, Create Epic, and Delete. If you want to see all epics, click on All Epics, if you want to create a new epic, click on Create Epic. A new window will show up, fill Epic title, Epic description, and Save Changes.You will then be redirected to the Epics page.
  </li>
  </ul>
  <br>
  
<b>4-Creating a Product Backlog</b>
<ul>
  <li>
	In order to create a New Product Backlog, you first must have and epic, and a client to which that particular epic belongs. So, on the Epics page, choose particular epic, click on three vertical dots in the bottom left, and you will see the menu with three options: All Product Backlogs, New Product Backlog, and Delete. Click on New Product Backlog.Fill out the window that shows up, and click on Save Shanges.
  </li>
</ul>
<br>


<b>5-Creating a task </b>
<p>
	You can create a task either from landing Scrum Board first page or from Sprint Backlog, after you move your Product Backlog to Sprint Backlog.
	To create a task from your landing page:
  </p>
  <ul>
  <li>Click on three vertical dots near TODO column 	 title, and click on New Item.</li>
	<li>A window apears, and the first three "must" options to select are Client, Epic, and Product Backlog. If you 			have not created each one of them before, go ahead and create them now, in the following sequence, 			Client, Epic, Product 	 Backlog. Then continue to specify task details and description.</li>
</ul>
<br>
	To create a task from Scrum Board:
  <ul>
  <li>
		In the first column to the left you will see all the Product Backlogs that you moved from Sprint Planning.
Pick one Product Backlog that you want to add a task to, and click on three vertical dots to the bottom left, then click on New Sprint Item.
  </li>
</ul>
<br>

<b>6-Moving a task to QA</b>
<ul>
<li>
  On landing Scrum Board page, click on three vertical dots on the bottom left of a task, and move it to QA accordingly. If the task is in TODO, click on INPROGRESS, and then click on QA, once it is in progress.
  </li>
</ul>
  <br>
  
<b>7-Changing QA Owner </b>

<ul>
 <li>If you want to change the owner of QA. Once the task is in QA, click on three vertical dots at the bottom left, and then click on Change Owner.  A window will appear with the list of team members who are in the same team as you are. Select one member and click on Save Changes. He or she in his or her turn will see the task in his or her TODO items once they log in.
  </li>
</ul>  
  </p>
  <p>
  <h5>How did Agile evolve - Useful Links</h5>
  
  
  <u><b>Agile Basics</b></u>
<ul>
  <li><i>Scrum Life Cycle:</i> https://en.wikipedia.org/wiki/Scrum_(software_development)
</li>
  <li><i>Waterfall Tree Cartoon:</i> https://www.tamingdata.com/2010/07/08/the-project-management-tree-swing-cartoon-past-and-present/</li>
</ul>

<u><b>Proof That Agile Works</b></u>

<ul>
  <li><i>P-80 Shooting Star:</i> https://en.wikipedia.org/wiki/Lockheed_P-80_Shooting_Star</li>
  <li><i>eROI Process Diagram:</i> https://image1.slideserve.com/1599299/slide3-n.jpg</li>
  <li><i>Condor Cluster Rack Image:</i> https://www.zdnet.com/article/what-the-dods-playstation-powered-condor-cluster-means-for-the-future-of-supercomputing/
  </li>
  <li><i>Condor Cluster Quotes and Additional Images:</i> https://www.youtube.com/watch?v=oumrKPLTMnk</li>
  <li><i>NASA Shoemaker Spacecraft:</i> https://www.youtube.com/watch?v=BZASB7RLhG8</li>
  <li><i>NASA Stardust Mission:</i> https://stardust.jpl.nasa.gov/home/index.html </li>
</ul>


<u><b>Evolution of Scrum</b></u>

<ul>
  <li><i>Edward Deming Image:</i> https://i.pinimg.com/736x/98/bc/1f/98bc1f7c7ce266dd7e2fe796be001285--teacher-w-edwards-deming.jpg</li>
  <li><i>PDCA Cycle:</i> http://texample.net/tikz/examples/pdca-cycle/</li>
  <li><i>Taichii Ohno:</i> https://c1.staticflickr.com/9/8110/8472007819_485415e875_b.jpg</li>
  <li><i>Kanban Board:</i> https://www.flickr.com/photos/kanban_tool/15817131058</li>
  <li><i>Eli Goldratt Image:</i> https://en.wikipedia.org/wiki/Eliyahu_M._Goldratt</li>
  <li><i>Bottleneck Image:</i> https://commons.wikimedia.org/wiki/File:Jam-before-Bottleneck.png</li>
  <li><i>Five Focusing Steps (e.g. POOGI):</i> https://www.tocinstitute.org/five-focusing-steps.htm</li>
</ul>


<u><b>NetFlix case study</b></u>
<p>KEYNOTE: Velocity and Volume (or Speed Wins) by Adrian Cockcroft</p>
<ul>
  <li>https://www.youtube.com/watch?v=wyWI3gLpB8o</li>
</ul>


<p>Link to the slides that Adrian Cockroft used at Flowcon:</p>
<ul>
  <li>http://flowcon.org/dl/flowcon-sanfran-2013/slides/AdrianCockcroft_VelocityAndVolumeorSpeedWins.pdf</li>
</ul>
<br>

<u><b>18F case study</b></u>

<ul>
  <li><i>18F Video Testimonial:</i> https://www.youtube.com/watch?v=lNSmF7-xisU</li>
  <li><i>18F Agile techniques and capabilities, called the "Toolkit:"</i> https://modularcontracting.18f.gov/strategies/</li>
</ul>



<u><b>Simple PM methods</b></u>


<ul>
  <li>Idea (person and light bulb): https://commons.wikimedia.org/wiki/File:Pictofigo_-_Idea.png</li>
  <li>Business Case: https://en.wikipedia.org/wiki/Use_case</li>
    <li>Deal Image (handshake): https://www.pexels.com/photo/two-person-shaking-each-others-hands-872957/</li>
  <li>Waterfall Model: https://commons.wikimedia.org/wiki/File:Waterfall_model.svg</li>
    <li>Factory Image: https://cdn.pixabay.com/photo/2018/04/16/09/12/factory-3323978_1280.png</li>
  <li>Tube-in-Trashcan: https://www.1001freedownloads.com/free-clipart/tv-in-trash</li>
    <li>Scrum Process: https://en.wikipedia.org/wiki/Scrum_(software_development)</li>
  <li>Agile Cycle (2 Week, 24 Hr): https://commons.wikimedia.org/wiki/File:Scrum_process.svg</li>
    <li>Development-Operations (DevOps): https://en.wikipedia.org/wiki/DevOps</li>
  <li>Plan-Do-Check-Act Cycle: https://www.flickr.com/photos/jurgenappelo/6943419159</li>
  <li>Kanban Board: https://commons.wikimedia.org/wiki/Category:Kanban#/media/File:Kanban_board_example.jpg</li>
  <li>Caution Icon: https://pixabay.com/en/attention-warning-sign-danger-303861/</li>
  <li>Question Mark Icon: https://svgsilh.com/4caf50/image/434152.html</li>
  
</ul>
<br>
<p>Fast car is  clipart</p>
<ul>
  <li>Work Breakdown Structure: https://en.wikipedia.org/wiki/Work_breakdown_structure</li>
</ul>  
  
  </p>
  <p>
	<h5>Images</h5>
	<p align="center">
  <img src="/Images/MDfiles/Infographic-Lite-Scrum.png" alt="infographic"/>
</p>
<p align="center">
  <img src="/Images/MDfiles/Scrum_Board.png" alt="scrum-board"/>
</p>
<p align="center">
  <img src="/Images/MDfiles/iPad_Lite_Scrum.png" alt="ipadv"/>
</p>
<p align="center">
  <img src="/Images/MDfiles/iPad_Lite_Scrum_h.png" alt="ipadh"/>
</p>

</p>
