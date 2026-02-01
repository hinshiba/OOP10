_digit_dec:
	subu	$sp,$sp,8
	sw	$fp,0($sp)
	move	$fp,$sp
	sw	$a0,8($fp)
	sw	$a1,12($fp)