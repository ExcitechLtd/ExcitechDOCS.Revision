Public Class CustomActions

    <CustomAction()>
    Public Shared Function InstallDocumentRevision(ByVal session As Session) As ActionResult
        session.Log("Begin InstallDocumentRevision")

        Dim _fToInstall As List(Of FeatureInfo) = session.Features.Where(Function(f)
                                                                             Return f.RequestState = InstallState.Local
                                                                         End Function).ToList

        For Each _ft As FeatureInfo In _fToInstall
            Select Case _ft.Name
                Case "featureRevitModules"

                    ''map the dll to the assemblies path
                    session.Log("Installing Revit Modules")
                    InstallRevitModule(session)
                Case "featureDocRevision"
                    ''spin up the installer exe
                    session.Log("Installing MFiles Server Components")
                    InstallServerComp(session)
                Case "featureDocumentation"
            End Select
        Next

        Return ActionResult.Success
    End Function

    Private Shared Sub InstallRevitModule(session As Session)
        Dim exDocsProgramData = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Excitech Docs")
        Dim filePath = IO.Path.Combine(exDocsProgramData, "Revit\Assemblies\loadLib.txt")
        Dim folderPath As String = IO.Path.GetDirectoryName(filePath)
        session.Log("Checking for path: " & folderPath)

        If Not IO.Directory.Exists(folderPath) Then
            session.Log("Createing path...")
            IO.Directory.CreateDirectory(folderPath)
        End If

        Dim dllPath As String = IO.Path.Combine(exDocsProgramData, "Revit\Assemblies\ExcitechDOCS.Revision.dll")
        session.Log("Checking for existing Revision dll: " & dllPath)
        IO.File.Delete(dllPath)

        If Not IO.File.Exists(dllPath) Then
            ''copy the dlls from the intall path
            Dim installDIR As String = session("dirRevit")
            ''copy all the dll files from the installdir to the folder path
            Dim _drInfo As New IO.DirectoryInfo(installDIR)
            For Each _file In _drInfo.GetFiles("*.dll")
                Dim _dstFile As String = IO.Path.Combine(folderPath, _file.Name)
                Try
                    IO.File.Copy(_file.FullName, _dstFile, True)
                Catch ex As Exception
                    session.Log("Error:")
                    session.Log(ex.ToString)
                End Try
            Next
        End If

        ''open the file
        Dim loadfile As New List(Of String)
        If IO.File.Exists(filePath) Then
            Using sr As New IO.StreamReader(filePath)
                While sr.Peek <> -1
                    loadfile.Add(sr.ReadLine.Trim.ToUpperInvariant)
                End While
            End Using
        End If

        Using _sw As New IO.StreamWriter(filePath, True)
            If Not loadfile.Contains("MSCORLIB") Then _sw.WriteLine("mscorlib")
            If Not loadfile.Contains("SYSTEM") Then _sw.WriteLine("system")
            If Not loadfile.Contains("SYSTEM.CORE") Then _sw.WriteLine("system.core")
            If Not loadfile.Contains(dllPath.ToUpperInvariant) Then _sw.WriteLine(dllPath)
        End Using

    End Sub

    Private Shared Sub InstallServerComp(session As Session)
        ''dirServer
        Dim installDIR As String = session("dirServer")
        Dim svrInstaller As String = IO.Path.Combine(installDIR, "ExcitechDOCS.Application.Installer.exe")
        If Not IO.File.Exists(svrInstaller) Then Throw New Exception("Unable to find Server Installer: ExcitechDOCS.Application.Installer.exe")

        Try
            Dim _proc As New Process
            ''_proc.StartInfo.Verb = "runas" ''must runas otherwise the process will break out of our impersonated context and start as the system
            _proc.StartInfo.Arguments = " " + ChrW(34) + "ExcitechDOCS document Revision" + ChrW(34) + " " + ChrW(34) + "ExDOCS_Revision_History.mfappx" + ChrW(34) + " " + ChrW(34) + "0F2B45EE-0DDE-4DAF-90B9-5BEAFD91F462" + ChrW(34)
            _proc.StartInfo.FileName = svrInstaller
            _proc.StartInfo.ErrorDialog = True
            _proc.Start()
            _proc.WaitForExit()
        Catch ex As Exception
            Throw New Exception("Unable to install server components " + vbCrLf + ex.ToString)
        End Try

    End Sub

    <CustomAction()>
    Public Shared Function UnInstallDocumentRevision(ByVal session As Session) As ActionResult
        session.Log("Begin UnInstallDocumentRevision")

        Dim _fToInstall As List(Of FeatureInfo) = session.Features.Where(Function(f)
                                                                             Return f.RequestState = InstallState.Absent
                                                                         End Function).ToList

        For Each _ft As FeatureInfo In _fToInstall
            Select Case _ft.Name
                Case "featureRevitModules"
                    ''remove the dll path from the load file and remove the files from the revit directory
                    UninstallRevitmodules(session)
                Case "featureDocRevision"
                Case "featureDocumentation"
            End Select
        Next

        Return ActionResult.Success
    End Function

    Private Shared Sub UninstallRevitmodules(session As Session)
        Dim exDocsProgramData = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Excitech Docs")
        Dim filePath = IO.Path.Combine(exDocsProgramData, "Revit\Assemblies\loadLib.txt")
        Dim dllPath As String = IO.Path.Combine(exDocsProgramData, "Revit\Assemblies\ExcitechDOCS.Revision.dll")

        ''open the file
        Dim loadfile As New List(Of String)
        If IO.File.Exists(filePath) Then
            Using sr As New IO.StreamReader(filePath)
                While sr.Peek <> -1
                    loadfile.Add(sr.ReadLine.Trim)
                End While
            End Using
        End If

        ''delete the file
        IO.File.Delete(filePath)

        ''write the lines backto the file removing the ones we want to unistall
        Using _sw As New IO.StreamWriter(filePath)
            For Each _str As String In loadfile
                If Not _str.ToUpperInvariant.EndsWith("EXCITECHDOCS.REVISION.DLL") Then
                    _sw.WriteLine(_str)
                End If
            Next
        End Using

        ''delete the dll 
        IO.File.Delete(dllPath)
    End Sub

End Class
