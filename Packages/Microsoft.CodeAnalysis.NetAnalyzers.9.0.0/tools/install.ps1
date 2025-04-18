﻿param($installPath, $toolsPath, $package, $project)

$analyzersDir = Join-Path (Split-Path -Path $toolsPath -Parent) "analyzers"
if (-Not (Test-Path $analyzersDir))
{
    return
}

$analyzersPaths = Join-Path ( $analyzersDir ) * -Resolve

foreach($analyzersPath in $analyzersPaths)
{
    # Install the language agnostic analyzers.
    if (Test-Path $analyzersPath)
    {
        foreach ($analyzerFilePath in Get-ChildItem $analyzersPath -Filter *.dll)
        {
            if($project.Object.AnalyzerReferences)
            {
                $project.Object.AnalyzerReferences.Add($analyzerFilePath.FullName)
            }
        }
    }
}

# $project.Type gives the language name like (C# or VB.NET)
$languageFolder = ""
if($project.Type -eq "C#")
{
    $languageFolder = "cs"
}
if($project.Type -eq "VB.NET")
{
    $languageFolder = "vb"
}
if($languageFolder -eq "")
{
    return
}

foreach($analyzersPath in $analyzersPaths)
{
    # Install language specific analyzers.
    $languageAnalyzersPath = join-path $analyzersPath $languageFolder
    if (Test-Path $languageAnalyzersPath)
    {
        foreach ($analyzerFilePath in Get-ChildItem $languageAnalyzersPath -Filter *.dll)
        {
            if($project.Object.AnalyzerReferences)
            {
                $project.Object.AnalyzerReferences.Add($analyzerFilePath.FullName)
            }
        }
    }
}
# SIG # Begin signature block
# MIIpKwYJKoZIhvcNAQcCoIIpHDCCKRgCAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCCh1voBODYpfCet
# zwmmor0XvG3ngI9/l8E/7NCzPBjyWaCCDfQwggZyMIIEWqADAgECAhMzAAAD0toZ
# Fl1tx0mvAAAAAAPSMA0GCSqGSIb3DQEBDAUAMH4xCzAJBgNVBAYTAlVTMRMwEQYD
# VQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNpZ25p
# bmcgUENBIDIwMTEwHhcNMjQwMjIyMjAyNTUyWhcNMjUwMjE5MjAyNTUyWjBjMQsw
# CQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9u
# ZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMQ0wCwYDVQQDEwQuTkVU
# MIIBojANBgkqhkiG9w0BAQEFAAOCAY8AMIIBigKCAYEAt2oShTpbhBjAB7H8x0eR
# ZVb8oThSm3lULJc6gUqIYKR8jg/1UJ3TCIZDEP5YrrIv4SiP9HGkU7LhDi39ZuIl
# 7OhQsMpq+bFM1GnfPRmQl8ZHmSxgGgzQwi0h4B9urEy6hbrDSrkedwLEmLTH0wla
# 6EuJBkpUl2VjdCLHFr89WYFj09vM9qcGFO2Ex4fTdJxuJ4B4MlxMhtYjLFasiVrB
# kjCCSQrdtW4l762ps0g8qZFKZBYZ4MDOR5PMUR6ErZg/MeUGV67zKnVnRrvJ5w1a
# HfCXi7HIbrbpgFBDjbFwWGt4DmrZUESCXSeUXbgVrYlW8gMfxOXZG2vcAul59Z61
# geytB3f8lLTbDsZHYUqw/SJKWBJtpskJXoEguYbH9aW2ZMrEVnDme6ENo0WQGrhS
# oQExEBURVjysnb0WCpmNOzlgJMab5iSNJ5FyIz5BhqEBzpaFpdQJnSGVYLJZhJzu
# CDg1u8fTZ4c9R+CjdQBaTFAZGcvVAbYafpdLCO4Ua53rAgMBAAGjggGCMIIBfjAf
# BgNVHSUEGDAWBgorBgEEAYI3TAgBBggrBgEFBQcDAzAdBgNVHQ4EFgQUA1bGKvRd
# p3PIdhdkOpAJEMIo7z8wVAYDVR0RBE0wS6RJMEcxLTArBgNVBAsTJE1pY3Jvc29m
# dCBJcmVsYW5kIE9wZXJhdGlvbnMgTGltaXRlZDEWMBQGA1UEBRMNNDY0MjIzKzUw
# MjA4NTAfBgNVHSMEGDAWgBRIbmTlUAXTgqoXNzcitW2oynUClTBUBgNVHR8ETTBL
# MEmgR6BFhkNodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpb3BzL2NybC9NaWND
# b2RTaWdQQ0EyMDExXzIwMTEtMDctMDguY3JsMGEGCCsGAQUFBwEBBFUwUzBRBggr
# BgEFBQcwAoZFaHR0cDovL3d3dy5taWNyb3NvZnQuY29tL3BraW9wcy9jZXJ0cy9N
# aWNDb2RTaWdQQ0EyMDExXzIwMTEtMDctMDguY3J0MAwGA1UdEwEB/wQCMAAwDQYJ
# KoZIhvcNAQEMBQADggIBAKUa9zGSKAiXDrTBYRMAHqD4gSxyp62CNIf2d0vg+j28
# KkazVXLOX0WxrRTd1S/uQLYOwFIAe9DMXeTSE53Ce8u6uTMJjBfxPbpNpw9SANPL
# whu40FP5yh/gCP2ae5eTeGRvH8G2Fr00aqscEH+vATPYWhvVa0Ky3hWwEWw+d+XQ
# FD7v0iS1hMHodV1nLWkGwhDX8bZxPc1DAx9tVX8vZK+T01Y+pj7Gx8b0uSptM3RU
# +Z7Xxe67mZ+O2A7sKBHQ62t6dI6JCNw8rx/UO6Sl10dH/S/M6iEWpO1sWzWAX/83
# cTgrQMUPe8AkBC3pldjIJKr6Pq6IRnZPCzopPV+CoP5GEbTiI1MZ4bQxwfaNelwO
# PyjmFO33vWlCrRB+fXKw8d0JxNo/unOW9cMediZV+B/wiyan6d9mWBx1QHXWTg9U
# NGSeJqnngd2h2LyftvCcpdrYWNsatmDtjLpzm5AdyobxZm2cxeDA9xkMQAqGo/IT
# uIy0mtRhW2WxCs/249xeL0TucAVxUma3WDnQys6a5d8BHrH1NE9zuumJVBADXlsQ
# E1EYDG8XDM+HymNFmNcnPtKP7wkPInVY4Z6PCRRV2CD9s+PP51fwpCajMrMX9V3o
# 2lr+/GEXhFIM5+F9uwltJph4yOU7tTWU57nWerifcE5YUk2dQUxkHgwWXfMlUeQr
# MIIHejCCBWKgAwIBAgIKYQ6Q0gAAAAAAAzANBgkqhkiG9w0BAQsFADCBiDELMAkG
# A1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQx
# HjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEyMDAGA1UEAxMpTWljcm9z
# b2Z0IFJvb3QgQ2VydGlmaWNhdGUgQXV0aG9yaXR5IDIwMTEwHhcNMTEwNzA4MjA1
# OTA5WhcNMjYwNzA4MjEwOTA5WjB+MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2Fz
# aGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENv
# cnBvcmF0aW9uMSgwJgYDVQQDEx9NaWNyb3NvZnQgQ29kZSBTaWduaW5nIFBDQSAy
# MDExMIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAq/D6chAcLq3YbqqC
# EE00uvK2WCGfQhsqa+laUKq4BjgaBEm6f8MMHt03a8YS2AvwOMKZBrDIOdUBFDFC
# 04kNeWSHfpRgJGyvnkmc6Whe0t+bU7IKLMOv2akrrnoJr9eWWcpgGgXpZnboMlIm
# Ei/nqwhQz7NEt13YxC4Ddato88tt8zpcoRb0RrrgOGSsbmQ1eKagYw8t00CT+OPe
# Bw3VXHmlSSnnDb6gE3e+lD3v++MrWhAfTVYoonpy4BI6t0le2O3tQ5GD2Xuye4Yb
# 2T6xjF3oiU+EGvKhL1nkkDstrjNYxbc+/jLTswM9sbKvkjh+0p2ALPVOVpEhNSXD
# OW5kf1O6nA+tGSOEy/S6A4aN91/w0FK/jJSHvMAhdCVfGCi2zCcoOCWYOUo2z3yx
# kq4cI6epZuxhH2rhKEmdX4jiJV3TIUs+UsS1Vz8kA/DRelsv1SPjcF0PUUZ3s/gA
# 4bysAoJf28AVs70b1FVL5zmhD+kjSbwYuER8ReTBw3J64HLnJN+/RpnF78IcV9uD
# jexNSTCnq47f7Fufr/zdsGbiwZeBe+3W7UvnSSmnEyimp31ngOaKYnhfsi+E11ec
# XL93KCjx7W3DKI8sj0A3T8HhhUSJxAlMxdSlQy90lfdu+HggWCwTXWCVmj5PM4Ta
# sIgX3p5O9JawvEagbJjS4NaIjAsCAwEAAaOCAe0wggHpMBAGCSsGAQQBgjcVAQQD
# AgEAMB0GA1UdDgQWBBRIbmTlUAXTgqoXNzcitW2oynUClTAZBgkrBgEEAYI3FAIE
# DB4KAFMAdQBiAEMAQTALBgNVHQ8EBAMCAYYwDwYDVR0TAQH/BAUwAwEB/zAfBgNV
# HSMEGDAWgBRyLToCMZBDuRQFTuHqp8cx0SOJNDBaBgNVHR8EUzBRME+gTaBLhklo
# dHRwOi8vY3JsLm1pY3Jvc29mdC5jb20vcGtpL2NybC9wcm9kdWN0cy9NaWNSb29D
# ZXJBdXQyMDExXzIwMTFfMDNfMjIuY3JsMF4GCCsGAQUFBwEBBFIwUDBOBggrBgEF
# BQcwAoZCaHR0cDovL3d3dy5taWNyb3NvZnQuY29tL3BraS9jZXJ0cy9NaWNSb29D
# ZXJBdXQyMDExXzIwMTFfMDNfMjIuY3J0MIGfBgNVHSAEgZcwgZQwgZEGCSsGAQQB
# gjcuAzCBgzA/BggrBgEFBQcCARYzaHR0cDovL3d3dy5taWNyb3NvZnQuY29tL3Br
# aW9wcy9kb2NzL3ByaW1hcnljcHMuaHRtMEAGCCsGAQUFBwICMDQeMiAdAEwAZQBn
# AGEAbABfAHAAbwBsAGkAYwB5AF8AcwB0AGEAdABlAG0AZQBuAHQALiAdMA0GCSqG
# SIb3DQEBCwUAA4ICAQBn8oalmOBUeRou09h0ZyKbC5YR4WOSmUKWfdJ5DJDBZV8u
# LD74w3LRbYP+vj/oCso7v0epo/Np22O/IjWll11lhJB9i0ZQVdgMknzSGksc8zxC
# i1LQsP1r4z4HLimb5j0bpdS1HXeUOeLpZMlEPXh6I/MTfaaQdION9MsmAkYqwooQ
# u6SpBQyb7Wj6aC6VoCo/KmtYSWMfCWluWpiW5IP0wI/zRive/DvQvTXvbiWu5a8n
# 7dDd8w6vmSiXmE0OPQvyCInWH8MyGOLwxS3OW560STkKxgrCxq2u5bLZ2xWIUUVY
# ODJxJxp/sfQn+N4sOiBpmLJZiWhub6e3dMNABQamASooPoI/E01mC8CzTfXhj38c
# bxV9Rad25UAqZaPDXVJihsMdYzaXht/a8/jyFqGaJ+HNpZfQ7l1jQeNbB5yHPgZ3
# BtEGsXUfFL5hYbXw3MYbBL7fQccOKO7eZS/sl/ahXJbYANahRr1Z85elCUtIEJmA
# H9AAKcWxm6U/RXceNcbSoqKfenoi+kiVH6v7RyOA9Z74v2u3S5fi63V4GuzqN5l5
# GEv/1rMjaHXmr/r8i+sLgOppO6/8MO0ETI7f33VtY5E90Z1WTk+/gFcioXgRMiF6
# 70EKsT/7qMykXcGhiJtXcVZOSEXAQsmbdlsKgEhr/Xmfwb1tbWrJUnMTDXpQzTGC
# Go0wghqJAgEBMIGVMH4xCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9u
# MRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRp
# b24xKDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNpZ25pbmcgUENBIDIwMTECEzMA
# AAPS2hkWXW3HSa8AAAAAA9IwDQYJYIZIAWUDBAIBBQCgga4wGQYJKoZIhvcNAQkD
# MQwGCisGAQQBgjcCAQQwHAYKKwYBBAGCNwIBCzEOMAwGCisGAQQBgjcCARUwLwYJ
# KoZIhvcNAQkEMSIEILHG5OyijfDOM5bxiJFmyVasy7d752HLA4qfvHm7QzF9MEIG
# CisGAQQBgjcCAQwxNDAyoBSAEgBNAGkAYwByAG8AcwBvAGYAdKEagBhodHRwOi8v
# d3d3Lm1pY3Jvc29mdC5jb20wDQYJKoZIhvcNAQEBBQAEggGAM4jKVPH5hoCi+uxs
# 2N89c0zPiDr/oSfASA3XmQLzMo9kej7J9YpD2al4Dha4CsDfvQxFna3bhoYfDjVA
# YO2gGpgi3eqKNYgMkfd9fXZLH3fudSK8ab4WQt0VdnDQqF0p6uFC10t5Nsl5F0dz
# eX8HLNV21QfjAnnxOuRge/bmcUMbQpA+xLTuQq7Kap3J3gJMndv91BOufBlV31Ck
# YT6R7c6Jk6REv1V2fi/t2WZ3DvKsHFQjv2tcdbOrpwjiLmsQDWyVdYxGnLpJvf38
# I5Qe8SOZw1WxhfydFoK1PqQLJ0Aj5Q4qIXkUXvuxIMoV35opZ8XlHNCEjoZ7TVEb
# 0LcCE9ZTs/C/PdCPtBbxSKEFdSbUWvYNJRsbeVBg2/qzJ4xRtnSymqsriBfQ50gy
# APOUvuRoF1GvVp1e+nLrZBr35LfwCCy1PrjrkcJRf6Y0rhISdsh3FaPB/uykk1gt
# HiTUqEM/5u2zg7o1sv9QkxRtkWRLJuBVnR/qxP0eyiOmnKcroYIXlzCCF5MGCisG
# AQQBgjcDAwExgheDMIIXfwYJKoZIhvcNAQcCoIIXcDCCF2wCAQMxDzANBglghkgB
# ZQMEAgEFADCCAVIGCyqGSIb3DQEJEAEEoIIBQQSCAT0wggE5AgEBBgorBgEEAYRZ
# CgMBMDEwDQYJYIZIAWUDBAIBBQAEIJ1kYRcwK+eSz/cDj8h7DjMbzxrekEZuke5o
# LDgZFd9FAgZm4t0YzXkYEzIwMjQxMDA4MjIwNTAxLjI2OFowBIACAfSggdGkgc4w
# gcsxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdS
# ZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xJTAjBgNVBAsT
# HE1pY3Jvc29mdCBBbWVyaWNhIE9wZXJhdGlvbnMxJzAlBgNVBAsTHm5TaGllbGQg
# VFNTIEVTTjo5MjAwLTA1RTAtRDk0NzElMCMGA1UEAxMcTWljcm9zb2Z0IFRpbWUt
# U3RhbXAgU2VydmljZaCCEe0wggcgMIIFCKADAgECAhMzAAAB5y6PL5MLTxvpAAEA
# AAHnMA0GCSqGSIb3DQEBCwUAMHwxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNo
# aW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29y
# cG9yYXRpb24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAyMDEw
# MB4XDTIzMTIwNjE4NDUxOVoXDTI1MDMwNTE4NDUxOVowgcsxCzAJBgNVBAYTAlVT
# MRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQK
# ExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xJTAjBgNVBAsTHE1pY3Jvc29mdCBBbWVy
# aWNhIE9wZXJhdGlvbnMxJzAlBgNVBAsTHm5TaGllbGQgVFNTIEVTTjo5MjAwLTA1
# RTAtRDk0NzElMCMGA1UEAxMcTWljcm9zb2Z0IFRpbWUtU3RhbXAgU2VydmljZTCC
# AiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIBAMJXny/gi5Drn1c8zUO1pYy/
# 38dFQLmR2IQXz1gE/r9GfuSOoyRnkRJ6Z/kSWLgIu1BVJ59GkXWPtLkssqKwxY4Z
# FotxpVsZN9yYjW8xEnW3MzAI0igKr+/LxYfxB1XUH8Bvmwr5D3Ii/MbDjtN9c8Tx
# GWtq7Ar976dafAy3TrRqQRmIknPVWHUuFJgpqI/1nbcRmYYRMJaKCQpty4CeG+Hf
# Ksxrz24F9p4dBkQcZCp2yQzjwQFxZJZ2mJJIGIDHKEdSRuSeX08/O0H9JTHNFmNT
# NYeD1t/WapnRwiIBYLQSMrs42GVB8pJEdUsos0+mXf/5QvheNzRi92pzzyA4tSv/
# zhP3/Ermvza6W9GnYDz9qv1wbhbvrnS4poDFECaAviEqAhfn/RogCxvKok5ro4gZ
# IX1r4N9eXUulA80pHv3axwXu2MPlarAi6J9L1hSIcy9EuOMqTRJIJX+alcLQGg+S
# Tlqx/GuslsKwl48dI4RuWknNGbNo/o4xfBFytvtNcVA6xOQq6qRa+9gg+9XMLrxQ
# z4yyQs+V3V6p044wrtJtt/a0ZJl/f6I7BZAxxZcH2DDmArcAhgrTxaQkm7LM+p+K
# 2C5t1EKZiv0JWw065b7AcNgaFyIkMXYuSuOQVSNRxdIgl31/ayxiK1n0K6sZXvgF
# Bx+vGO+TUvyO+03ua6UjAgMBAAGjggFJMIIBRTAdBgNVHQ4EFgQUz/7gmICfNjh2
# kR/9mWuHUrvej1gwHwYDVR0jBBgwFoAUn6cVXQBeYl2D9OXSZacbUzUZ6XIwXwYD
# VR0fBFgwVjBUoFKgUIZOaHR0cDovL3d3dy5taWNyb3NvZnQuY29tL3BraW9wcy9j
# cmwvTWljcm9zb2Z0JTIwVGltZS1TdGFtcCUyMFBDQSUyMDIwMTAoMSkuY3JsMGwG
# CCsGAQUFBwEBBGAwXjBcBggrBgEFBQcwAoZQaHR0cDovL3d3dy5taWNyb3NvZnQu
# Y29tL3BraW9wcy9jZXJ0cy9NaWNyb3NvZnQlMjBUaW1lLVN0YW1wJTIwUENBJTIw
# MjAxMCgxKS5jcnQwDAYDVR0TAQH/BAIwADAWBgNVHSUBAf8EDDAKBggrBgEFBQcD
# CDAOBgNVHQ8BAf8EBAMCB4AwDQYJKoZIhvcNAQELBQADggIBAHSh8NuT6WVaLVwL
# qex+J7km2nT2jpvoBEKm+0M+rYoU/6GL5Q00/ssZyIq5ySpcKYFMUiF8F4ZLG+Tr
# JyiR1CvfzXmkQ5phZOce9DT7yErLzqvUXit8G7igcHlxPLTxPiiGsb85gb8H+A2f
# PQ6Xq/u7+oSPPjzNdnpmXEobJnAqYplZoF3YNgTDMql0uQHGzoDp6dZlHSNj6rkV
# 1tXjmCEZMqBKvkQIA6csPieMnB+MirSZFlbANlChe0lJpUdK7aUdAvdgcQWKS6dt
# RMl818EMsvsa/6xOZGINmTLk4DGgsbaBpN+6IVt+mZJ89yCXkI5TN8xCfOkp9fr4
# WQjRBA2+4+lawNTyxH66eLZWYOjuuaomuibiKGBU10tox81Sq8EvlmJIrXOZoQsE
# n1r5g6MTmmZJqtbmwZufuJWQXZb0lAg4fq0ZYsUlLkezfrNqGSgeHyIP3rct4aNm
# qQW6wppRbvbIyP/LFN4YQM6givfmTBfGvVS77OS6vbL4W41jShmOmnOn3kBbWV6E
# /TFo76gFXVd+9oK6v8Hk9UCnbHOuiwwRRwDCkmmKj5Vh8i58aPuZ5dwZBhYDxSav
# wroC6j4mWPwh4VLqVK8qGpCmZ0HMAwao85Aq3U7DdlfF6Eru8CKKbdmIAuUzQrnj
# qTSxmvF1k+CmbPs7zD2Acu7JkBB7MIIHcTCCBVmgAwIBAgITMwAAABXF52ueAptJ
# mQAAAAAAFTANBgkqhkiG9w0BAQsFADCBiDELMAkGA1UEBhMCVVMxEzARBgNVBAgT
# Cldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29m
# dCBDb3Jwb3JhdGlvbjEyMDAGA1UEAxMpTWljcm9zb2Z0IFJvb3QgQ2VydGlmaWNh
# dGUgQXV0aG9yaXR5IDIwMTAwHhcNMjEwOTMwMTgyMjI1WhcNMzAwOTMwMTgzMjI1
# WjB8MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMH
# UmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSYwJAYDVQQD
# Ex1NaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EgMjAxMDCCAiIwDQYJKoZIhvcNAQEB
# BQADggIPADCCAgoCggIBAOThpkzntHIhC3miy9ckeb0O1YLT/e6cBwfSqWxOdcjK
# NVf2AX9sSuDivbk+F2Az/1xPx2b3lVNxWuJ+Slr+uDZnhUYjDLWNE893MsAQGOhg
# fWpSg0S3po5GawcU88V29YZQ3MFEyHFcUTE3oAo4bo3t1w/YJlN8OWECesSq/XJp
# rx2rrPY2vjUmZNqYO7oaezOtgFt+jBAcnVL+tuhiJdxqD89d9P6OU8/W7IVWTe/d
# vI2k45GPsjksUZzpcGkNyjYtcI4xyDUoveO0hyTD4MmPfrVUj9z6BVWYbWg7mka9
# 7aSueik3rMvrg0XnRm7KMtXAhjBcTyziYrLNueKNiOSWrAFKu75xqRdbZ2De+JKR
# Hh09/SDPc31BmkZ1zcRfNN0Sidb9pSB9fvzZnkXftnIv231fgLrbqn427DZM9itu
# qBJR6L8FA6PRc6ZNN3SUHDSCD/AQ8rdHGO2n6Jl8P0zbr17C89XYcz1DTsEzOUyO
# ArxCaC4Q6oRRRuLRvWoYWmEBc8pnol7XKHYC4jMYctenIPDC+hIK12NvDMk2ZItb
# oKaDIV1fMHSRlJTYuVD5C4lh8zYGNRiER9vcG9H9stQcxWv2XFJRXRLbJbqvUAV6
# bMURHXLvjflSxIUXk8A8FdsaN8cIFRg/eKtFtvUeh17aj54WcmnGrnu3tz5q4i6t
# AgMBAAGjggHdMIIB2TASBgkrBgEEAYI3FQEEBQIDAQABMCMGCSsGAQQBgjcVAgQW
# BBQqp1L+ZMSavoKRPEY1Kc8Q/y8E7jAdBgNVHQ4EFgQUn6cVXQBeYl2D9OXSZacb
# UzUZ6XIwXAYDVR0gBFUwUzBRBgwrBgEEAYI3TIN9AQEwQTA/BggrBgEFBQcCARYz
# aHR0cDovL3d3dy5taWNyb3NvZnQuY29tL3BraW9wcy9Eb2NzL1JlcG9zaXRvcnku
# aHRtMBMGA1UdJQQMMAoGCCsGAQUFBwMIMBkGCSsGAQQBgjcUAgQMHgoAUwB1AGIA
# QwBBMAsGA1UdDwQEAwIBhjAPBgNVHRMBAf8EBTADAQH/MB8GA1UdIwQYMBaAFNX2
# VsuP6KJcYmjRPZSQW9fOmhjEMFYGA1UdHwRPME0wS6BJoEeGRWh0dHA6Ly9jcmwu
# bWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3RzL01pY1Jvb0NlckF1dF8yMDEw
# LTA2LTIzLmNybDBaBggrBgEFBQcBAQROMEwwSgYIKwYBBQUHMAKGPmh0dHA6Ly93
# d3cubWljcm9zb2Z0LmNvbS9wa2kvY2VydHMvTWljUm9vQ2VyQXV0XzIwMTAtMDYt
# MjMuY3J0MA0GCSqGSIb3DQEBCwUAA4ICAQCdVX38Kq3hLB9nATEkW+Geckv8qW/q
# XBS2Pk5HZHixBpOXPTEztTnXwnE2P9pkbHzQdTltuw8x5MKP+2zRoZQYIu7pZmc6
# U03dmLq2HnjYNi6cqYJWAAOwBb6J6Gngugnue99qb74py27YP0h1AdkY3m2CDPVt
# I1TkeFN1JFe53Z/zjj3G82jfZfakVqr3lbYoVSfQJL1AoL8ZthISEV09J+BAljis
# 9/kpicO8F7BUhUKz/AyeixmJ5/ALaoHCgRlCGVJ1ijbCHcNhcy4sa3tuPywJeBTp
# kbKpW99Jo3QMvOyRgNI95ko+ZjtPu4b6MhrZlvSP9pEB9s7GdP32THJvEKt1MMU0
# sHrYUP4KWN1APMdUbZ1jdEgssU5HLcEUBHG/ZPkkvnNtyo4JvbMBV0lUZNlz138e
# W0QBjloZkWsNn6Qo3GcZKCS6OEuabvshVGtqRRFHqfG3rsjoiV5PndLQTHa1V1QJ
# sWkBRH58oWFsc/4Ku+xBZj1p/cvBQUl+fpO+y/g75LcVv7TOPqUxUYS8vwLBgqJ7
# Fx0ViY1w/ue10CgaiQuPNtq6TPmb/wrpNPgkNWcr4A245oyZ1uEi6vAnQj0llOZ0
# dFtq0Z4+7X6gMTN9vMvpe784cETRkPHIqzqKOghif9lwY1NNje6CbaUFEMFxBmoQ
# tB1VM1izoXBm8qGCA1AwggI4AgEBMIH5oYHRpIHOMIHLMQswCQYDVQQGEwJVUzET
# MBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMV
# TWljcm9zb2Z0IENvcnBvcmF0aW9uMSUwIwYDVQQLExxNaWNyb3NvZnQgQW1lcmlj
# YSBPcGVyYXRpb25zMScwJQYDVQQLEx5uU2hpZWxkIFRTUyBFU046OTIwMC0wNUUw
# LUQ5NDcxJTAjBgNVBAMTHE1pY3Jvc29mdCBUaW1lLVN0YW1wIFNlcnZpY2WiIwoB
# ATAHBgUrDgMCGgMVALNyBOcZqxLB792u75w97U0X+/BDoIGDMIGApH4wfDELMAkG
# A1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQx
# HjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEmMCQGA1UEAxMdTWljcm9z
# b2Z0IFRpbWUtU3RhbXAgUENBIDIwMTAwDQYJKoZIhvcNAQELBQACBQDqr6AZMCIY
# DzIwMjQxMDA4MTIxMjA5WhgPMjAyNDEwMDkxMjEyMDlaMHcwPQYKKwYBBAGEWQoE
# ATEvMC0wCgIFAOqvoBkCAQAwCgIBAAICLpgCAf8wBwIBAAICE0swCgIFAOqw8ZkC
# AQAwNgYKKwYBBAGEWQoEAjEoMCYwDAYKKwYBBAGEWQoDAqAKMAgCAQACAwehIKEK
# MAgCAQACAwGGoDANBgkqhkiG9w0BAQsFAAOCAQEAXPWCvHLBj3KlwOOKUlJLlf7x
# Jepa3ym2ZjGJVKHBYfcFBHLwMakJxKvyancQZVFEQdqvq0RGLR33NZkJ6iQH0wwj
# y8StDMXA0BpeTgsZyuuLGwGMqBMIiksvGKm1RPvRPeTBG1tooiJFoMPTv5Uadws8
# /8ISKXuNsB7YD6THNoyd7TOxJ7YmSfnWBBnC5PBCFh+mgaEjMer8GZ2tUqCQeQJ8
# h/aO0g4XqlzoVFAavyqJ0iTHVy/vsw2FeJ7SAj0z9V+oH4elUG1KPw+EEKJS59wU
# AYV+4joqKV01sJalF0Mewkb/LWeQ7ALf3BFD+2KEel1BR1NLBjZ5+e7K5E66TzGC
# BA0wggQJAgEBMIGTMHwxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9u
# MRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRp
# b24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAyMDEwAhMzAAAB
# 5y6PL5MLTxvpAAEAAAHnMA0GCWCGSAFlAwQCAQUAoIIBSjAaBgkqhkiG9w0BCQMx
# DQYLKoZIhvcNAQkQAQQwLwYJKoZIhvcNAQkEMSIEIOwOPzX/7QeBUlIHEJz9Xa8r
# F4j5a88szkCeRYdgdRJkMIH6BgsqhkiG9w0BCRACLzGB6jCB5zCB5DCBvQQg5TZd
# DXZqhv0N4MVcz1QUd4RfvgW/QAG9AwbuoLnWc60wgZgwgYCkfjB8MQswCQYDVQQG
# EwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwG
# A1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSYwJAYDVQQDEx1NaWNyb3NvZnQg
# VGltZS1TdGFtcCBQQ0EgMjAxMAITMwAAAecujy+TC08b6QABAAAB5zAiBCBaGhYA
# 0fhyDoPFcPnUA1smX8+NEAgLMjsHYbGgq/YH7DANBgkqhkiG9w0BAQsFAASCAgDB
# tSyY0IZRUX1855HwSGjl3ZpZ0amyag3ogDN2KQ3uKQ1qbKcYc4rUbr/9japovNys
# Cq6V7Z5tZ99hBBDfqBLJzFwaL4aKAgupamCT+jy5IKyLgndvT1lGTdusa3Mk1mrR
# lDssy0UtBPVvW7rum4CLZqDA+MVvtrCNz9IIK5T1sluy+6G9ZRZhsXj3MnBVTT1E
# oAzFbD759HagukiQjJPWAbIjpwvk4kYAvtYSYeikMLXvoyeOBgt/UJM7TkCRUQq/
# OKiEguykQYamK/kgdMY8itsZfYxzges5TebanQjJfI+cpkW/OYCw2WMcW6PrRkxE
# Ncd57QzYnRKPKBTu6bIO81ZFeN//nQHKk1nGLGLE9ZF0iPWhC5CBcYJQOe1ZPefs
# 6wLAGLnjvAwNJJfpwcPRmvUcsxTTmuLbyg8l2cdjiNJXLXCIfx2AWUoxWbp8hSsH
# TFbqxLjq/uu9AJgoRrm4SoRh5Nul+M708jhLaufWKs73OvWgCgDG1JwW1/gxBW/U
# DcdEDFXbv1mE/wtC6b4meXuctHVBJU5wt/LDrI4lY0SDFcnh4eaf06qdG9LTVh88
# K/R1BdNtnrvO48/lFmobi6mI/i3gJuwUC/CmybXdr1lKKq5NU+IVoXsosizDflCH
# 0r+/yoM9a9OShlyN3iRE2hrVntI8GwEDCKMukvkcAg==
# SIG # End signature block
