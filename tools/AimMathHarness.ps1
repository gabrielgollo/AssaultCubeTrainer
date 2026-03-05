param(
    [ValidateSet("deg", "rad")]
    [string]$AngleUnit = "deg",

    [ValidateSet("zup", "yup")]
    [string]$AxisMode = "zup",

    [double]$YawOffset = 90.0,
    [switch]$InvertPitch,
    [switch]$SwapAxisYZ
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Convert-ToAngleUnits {
    param(
        [double]$Radians,
        [string]$Unit
    )

    if ($Unit -eq "rad") {
        return $Radians
    }

    return $Radians * (180.0 / [Math]::PI)
}

function Clamp-Pitch {
    param([double]$Pitch)

    if ($Pitch -lt -90.0) { return -90.0 }
    if ($Pitch -gt 90.0) { return 90.0 }
    return $Pitch
}

function Normalize-Yaw {
    param([double]$Yaw)

    $result = $Yaw % 360.0
    if ($result -lt 0.0) {
        $result += 360.0
    }

    return $result
}

function Resolve-Axes {
    param(
        [double]$X,
        [double]$Y,
        [double]$Z,
        [bool]$SwapYZ
    )

    if ($SwapYZ) {
        return [pscustomobject]@{
            X = $X
            Y = $Z
            Z = $Y
        }
    }

    return [pscustomobject]@{
        X = $X
        Y = $Y
        Z = $Z
    }
}

function Get-AimAngles {
    param(
        [double]$PlayerX,
        [double]$PlayerY,
        [double]$PlayerZ,
        [double]$EnemyX,
        [double]$EnemyY,
        [double]$EnemyZ,
        [string]$Mode,
        [string]$Unit,
        [double]$Offset,
        [bool]$Invert,
        [bool]$SwapYZ
    )

    $player = Resolve-Axes -X $PlayerX -Y $PlayerY -Z $PlayerZ -SwapYZ $SwapYZ
    $enemy = Resolve-Axes -X $EnemyX -Y $EnemyY -Z $EnemyZ -SwapYZ $SwapYZ

    $dx = $enemy.X - $player.X
    $dy = $enemy.Y - $player.Y
    $dz = $enemy.Z - $player.Z

    if ($Mode -eq "yup") {
        $horizontal = [Math]::Sqrt(($dx * $dx) + ($dz * $dz))
        if ($horizontal -lt 0.001) {
            $horizontal = 0.001
        }

        $yawRadians = [Math]::Atan2($dz, $dx)
        $pitchRadians = [Math]::Atan2($dy, $horizontal)
    }
    else {
        $horizontal = [Math]::Sqrt(($dx * $dx) + ($dy * $dy))
        if ($horizontal -lt 0.001) {
            $horizontal = 0.001
        }

        $yawRadians = [Math]::Atan2($dy, $dx)
        $pitchRadians = [Math]::Atan2($dz, $horizontal)
    }

    $yaw = Convert-ToAngleUnits -Radians $yawRadians -Unit $Unit
    $pitch = Convert-ToAngleUnits -Radians $pitchRadians -Unit $Unit

    if ($Unit -eq "deg") {
        $yaw = Normalize-Yaw ($yaw + $Offset)
        if ($Invert) {
            $pitch = -$pitch
        }

        $pitch = Clamp-Pitch $pitch
    }
    else {
        $yaw += $Offset
        if ($Invert) {
            $pitch = -$pitch
        }
    }

    return [pscustomobject]@{
        Yaw = [Math]::Round($yaw, 4)
        Pitch = [Math]::Round($pitch, 4)
        HorizontalDistance = [Math]::Round($horizontal, 4)
        DeltaX = [Math]::Round($dx, 4)
        DeltaY = [Math]::Round($dy, 4)
        DeltaZ = [Math]::Round($dz, 4)
    }
}

$cases = @(
    [pscustomobject]@{ Name = "Front +X"; EnemyX = 10.0; EnemyY = 0.0; EnemyZ = 0.0 },
    [pscustomobject]@{ Name = "Right +Y"; EnemyX = 0.0; EnemyY = 10.0; EnemyZ = 0.0 },
    [pscustomobject]@{ Name = "Back -X"; EnemyX = -10.0; EnemyY = 0.0; EnemyZ = 0.0 },
    [pscustomobject]@{ Name = "Left -Y"; EnemyX = 0.0; EnemyY = -10.0; EnemyZ = 0.0 },
    [pscustomobject]@{ Name = "Above +Z"; EnemyX = 10.0; EnemyY = 0.0; EnemyZ = 10.0 },
    [pscustomobject]@{ Name = "Below -Z"; EnemyX = 10.0; EnemyY = 0.0; EnemyZ = -10.0 }
)

$rows = foreach ($case in $cases) {
    $angles = Get-AimAngles `
        -PlayerX 0.0 `
        -PlayerY 0.0 `
        -PlayerZ 0.0 `
        -EnemyX $case.EnemyX `
        -EnemyY $case.EnemyY `
        -EnemyZ $case.EnemyZ `
        -Mode $AxisMode `
        -Unit $AngleUnit `
        -Offset $YawOffset `
        -Invert $InvertPitch.IsPresent `
        -SwapYZ $SwapAxisYZ.IsPresent

    [pscustomobject]@{
        Case = $case.Name
        AxisMode = $AxisMode
        AngleUnit = $AngleUnit
        YawOffset = $YawOffset
        InvertPitch = $InvertPitch.IsPresent
        SwapAxisYZ = $SwapAxisYZ.IsPresent
        Yaw = $angles.Yaw
        Pitch = $angles.Pitch
        HorizontalDistance = $angles.HorizontalDistance
        DeltaX = $angles.DeltaX
        DeltaY = $angles.DeltaY
        DeltaZ = $angles.DeltaZ
    }
}

$rows | Format-Table -AutoSize
