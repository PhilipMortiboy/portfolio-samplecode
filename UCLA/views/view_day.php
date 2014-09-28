<div id="content">
	<div class="padding">
	<h1><?php echo $day; ?></h1>
	<?php 
		if($pre_selected != null)
		{?>
			<p>You have already been selected for <b><?php echo ''.$pre_selected.''; ?>.</b></p>
			<p>Click the Next button to move on</p>
		<?php }else{ ?>
		
			<p>Select from one of the following options test:</p>
			
		<?php 
			
			echo validation_errors();
			echo form_open('student/day/'.$dayNum.'');
		?>
			<select name="selection" size="10">
				<?php
					foreach ($activity as $activity_item){ ?>
					<option><?php echo ''.$activity_item['name'].''; ?></option>
				<?php } ?>
			</select>
			<
		<?php } ?>
	
		<input type="submit" name="submit" value="Next" />
	</form>
	</div><!--Closes padding-->
</div><!--Closes content-->