<div id="content">
	<div class="padding">
	<h1>Summary</h1>
		<p><b><?php echo $name; ?></b>, you have chose the following activities for your Self Directed Curriculum</p>
		<p>Monday: <b><?php echo $summary['actDay1']; ?></b> <a href="<?php echo base_url('student/edit/1'); ?>"> Change</a></br>
		Wednesday: <b><?php echo $summary['actDay2']; ?></b> <a href="<?php echo base_url('student/edit/2'); ?>"> Change</a></br>
		Thursday: <b><?php echo $summary['actDay3']; ?></b> <a href="<?php echo base_url('student/edit/3'); ?>"> Change</a></br>
		Friday: <b><?php echo $summary['actDay4']; ?></b> <a href="<?php echo base_url('student/edit/4'); ?>"> Change</a></br>
		</p>
	<?php 
		if($complete == 1)
		{
	?>
		<p>If you're happy with these selections, click the finish button. If not, you can click 'Change' next to any of your options to chose a new one.</p>
		<a href="<?php echo base_url('student/finish'); ?>">
					<input type="submit" value="Finish">
		</a>
	<?php 
		}else{
	?>
		<p>You must select <b>Personal Study</b> on at least one day. Click 'Change' next to any day to go back and pick Personal Study from the list of avalible acitivites. </p>
	<?php } ?>
	</div><!--Closes padding-->
</div><!--Closes content-->