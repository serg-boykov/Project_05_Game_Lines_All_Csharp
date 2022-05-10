<?php
    $text = file_get_contents('map.txt');
    $line = str_replace("\r", '', $text);
    $line = str_replace("\n", '', $line);
    $map = [];
    
    for ($nr = 0; $nr < 81; $nr++) {
        $map[$nr % 9][floor($nr / 9)] = $line[$nr];
    }
	echo json_encode($map);
?>