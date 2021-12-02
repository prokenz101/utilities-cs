F9::
    Send, ^a
    Send, ^c
    Sleep, 150
    Run, bin\Release\netcoreapp3.1\utilities-cs.exe %Clipboard%
    Send, {Esc}
Return

+F9::
    Send, ^c
    Sleep, 150
    Run, bin\Release\netcoreapp3.1\utilities-cs.exe %Clipboard%
Return
