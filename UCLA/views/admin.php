<div id="content">
	<div class="padding">
	<h1>Welcome to the admin page</h1>
	<p>What would you like to do?</p>
	<ul>
	<li><?php echo anchor('teacher/updateStudentsPage', 'Update the list of students', 'title="Apps"');?></li>
	<li><?php echo anchor('teacher/updateActivitiesPage', 'Update the list of Self Directed Curriculum activities', 'title="Apps"');?></li>
	<li><?php echo anchor('teacher/downloadStudentList', 'Download a list of what activities the students have pick', 'title="Apps"');?></li>
	<li><?php echo anchor('teacher/notCompleted', 'Check which students have not yet completed thier choices', 'title="Apps"');?></li>
	</div><!--Closes padding-->
</div><!--Closes content-->