<div id="content">
	<div class="padding">
	<table>
	<tr>
		<th>Name</th>
		<th>Year</th>
		<th>Form</th>
	</tr>
	<?php
		foreach ($student as $student_item){ ?>
		<tr>
			<td><?php echo ''.$student_item['fName'].' '.$student_item['lName'].''; ?></td>
			<td><?php echo ''.$student_item['year'].''; ?></td>
			<td><?php echo ''.$student_item['form'].''; ?></td>
		</tr>
	<?php } ?>
	</table>
	</div><!--Closes padding-->
</div><!--Closes content-->