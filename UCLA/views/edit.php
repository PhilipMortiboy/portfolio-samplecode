<div id="content">
	<div class="padding">
	<h1><?php echo $day; ?></h1>
	<?php 
		if($pre_selected != null)
		{
			if($dayNum == 4){?>
				<p>You have already been selected for <b><?php echo ''.$pre_selected.''; ?>.</b></p>
			<p>Click the Next button to move on</p>
			<a href="<?php echo base_url('student/checkCompletion'); ?>">
				<input type="submit" value="Next">
			</a>
			
			<?php }else{ ?>
			<p>You have already been selected for <b><?php echo ''.$pre_selected.''; ?>.</b></p>
			<p>Click the Next button to move on</p>
			<a href="<?php echo base_url('student/day/'.$dayNext.''); ?>">
				<input type="submit" value="Next">
			</a>
		<?php 
			}
		
		}else{ ?>
		
			<p>Select from one of the following options:</p>
			
		<?php 
			
			echo validation_errors();
			echo form_open('student/edit/'.$dayNum.'');
		?>
			<select name="selection" size="10">
				<?php
					foreach ($activity as $activity_item){ ?>
					<option><?php echo ''.$activity_item['name'].''; ?></option>
				<?php } ?>
			</select>
			<input type="submit" name="submit" value="Edit" />
		<?php } ?>
	
	</form>
	</div><!--Closes padding-->
</div><!--Closes content-->