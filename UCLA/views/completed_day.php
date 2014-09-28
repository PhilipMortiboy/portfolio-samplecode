<div id="content">
	<div class="padding">
	<h1><?php echo $day; ?></h1>
	<p>You have selected <b><?php echo $selection; ?></b> for your acitity on <b><?php echo $day; ?></b>.
	<p>Click the next button to move on</p>
	
	<!--<a href="<?php echo base_url('student/day/'.$dayNum.''); ?>">
		<input type="submit" name="submit" value="Back" />
	</a>-->
	<a href="<?php echo base_url('student/day/'.$dayNext.''); ?>">
		<input type="submit" name="submit" value="Next" />
	</a>
	</div><!--Closes padding-->
</div><!--Closes content-->