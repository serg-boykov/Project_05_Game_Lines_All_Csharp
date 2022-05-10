<?php
    $map = $_GET['map']; // 00000 ... 0 <= 81
    // http://lines/save-map.php?map=000000000000000000000000000000002004000000000000000000000000000200000000000000000
	
    if (strlen($map) != 81) {
        die('81');
    }
    
    $text = '';
    
    for ($nr = 0; $nr < 81; $nr += 9) {
        $text.= substr($map, $nr, 9). "\r\n";
    }
    
    file_put_contents('map.txt', $text);
    echo "saved";
?>