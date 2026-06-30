<?php

session_start();

if(!isset($_SESSION["documento"]))
{
    header("Location: ingreso.html");
    exit();
}

require("conexion.php");


$documento = $_SESSION["documento"];


// Buscar la tarjeta del usuario

$sqlTarjeta =
"
SELECT num_cuenta,
       numero_tarjeta
FROM tarjetas
WHERE dni_titular = ?
";

$stmt = $conexion->prepare($sqlTarjeta);

$stmt->bind_param("s",$documento);

$stmt->execute();

$resultadoTarjeta = $stmt->get_result();

$tarjeta = $resultadoTarjeta->fetch_assoc();

$numCuenta = $tarjeta["num_cuenta"];

$stmt->close();


// Buscar la liquidación más reciente

$sqlActual =
"
SELECT *
FROM liquidaciones
WHERE num_cuenta = ?
ORDER BY periodo DESC
LIMIT 1
";

$stmt = $conexion->prepare($sqlActual);

$stmt->bind_param("i",$numCuenta);

$stmt->execute();

$actual = $stmt->get_result();


// Buscar historial

$sqlHistorial =
"
SELECT *
FROM liquidaciones
WHERE num_cuenta = ?
ORDER BY periodo DESC
";

$stmt2 = $conexion->prepare($sqlHistorial);

$stmt2->bind_param("i",$numCuenta);

$stmt2->execute();

$historial = $stmt2->get_result();

?>

<!DOCTYPE html>

<html>

<head>

<meta charset="UTF-8">

<title>Resumen</title>

</head>

<body>

<h1>Bienvenido <?php echo $_SESSION["usuario"]; ?></h1>

<hr>

<h2>Última liquidación</h2>

<?php

if($actual->num_rows>0)
{
    $fila = $actual->fetch_assoc();

    echo "Período: ".$fila["periodo"]."<br>";
    echo "Vencimiento: ".$fila["fecha_vencimiento"]."<br>";
    echo "Total: $".$fila["importe_total"]."<br>";
}
else
{
    echo "No existen liquidaciones.";
}

?>

<hr>

<h2>Historial</h2>

<table border="1">

<tr>

<th>Período</th>

<th>Total</th>

<th>Vencimiento</th>

</tr>

<?php

while($fila=$historial->fetch_assoc())
{
?>

<tr>

<td><?php echo $fila["periodo"]; ?></td>

<td><?php echo $fila["importe_total"]; ?></td>

<td><?php echo $fila["fecha_vencimiento"]; ?></td>

</tr>

<?php
}
?>

</table>

</body>

</html>

<?php

$stmt->close();
$stmt2->close();
$conexion->close();

?>