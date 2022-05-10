<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Stream Lines Game</title>
	<script>

        function showBox(x, y, ball) {
            document.getElementById('b' + x + y).src = ball + '.png';
        }

        function getGame() {
            let map = callPHP('/load-map.php');
            for (x = 0; x < 9; x++) {
                for (y = 0; y < 9; y++) {
                    showBox(x, y, map[x][y]);
                }
            }
        }

        function callPHP(url) {
            let request = new XMLHttpRequest();
            request.open('GET', url, false);
            request.send(null);
            let text = request.responseText;
            console.log(text);
            return JSON.parse(text);
        }

    </script>
</head>
<body>
    <table>
        <?php for ($y = 0; $y < 9; $y++) { ?>

        <tr>
            <?php for ($x = 0; $x < 9; $x++) { ?>

            <td><img id='b<?= $x . $y ?>' src='0.png' alt='' /></td>

            <?php } ?>
        </tr>

        <?php } ?>
    </table>
	<script>
        setInterval(getGame, 1000);
    </script>
</body>
</html>