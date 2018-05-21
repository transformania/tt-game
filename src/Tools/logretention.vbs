Cleanup "c:\inetpub\logs\LogFiles", 30
Cleanup "c:\inetpub\ttgamede", 30

Sub Cleanup(sLogFolder, iMaxAge)
    Set objFSO = CreateObject("Scripting.FileSystemObject")
    Set colFolder = objFSO.GetFolder(sLogFolder)
    For Each colSubfolder in colFolder.SubFolders
        Set objFolder = objFSO.GetFolder(colSubfolder.Path)
        Set colFiles = objFolder.Files
        For Each objFile in colFiles
            iFileAge = now-objFile.DateCreated
            if iFileAge > (iMaxAge+1)  then
                objFSO.deletefile objFile, True
            End If
        Next
    Next
End Sub
