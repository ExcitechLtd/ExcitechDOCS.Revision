Imports System
Imports System.ComponentModel
Imports System.Configuration.Install

<RunInstaller(True)>
Public Class CustomSetupActions
    Inherits Installer

#Region " Private "
    Private _revit As Boolean
    Private _client As Boolean
    Private _target As String
#End Region

    Public Overrides Sub Install(stateSaver As IDictionary)
        MyBase.Install(stateSaver)

        _revit = Not String.IsNullOrWhiteSpace(Context.Parameters("revit")) AndAlso Context.Parameters("revit") = 1
        _client = Not String.IsNullOrWhiteSpace(Context.Parameters("client")) AndAlso Context.Parameters("client") = 1
        _target = Context.Parameters("targetdir").Trim

        If _revit Then
            ''merge in the assemblies load file
            ''see if there is a revit
            Dim exDocsProgramData = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Excitech Docs")
            Dim filePath = IO.Path.Combine(exDocsProgramData, "Revit\Assemblies\loadLib.txt")
            Dim dllPath As String = IO.Path.Combine(exDocsProgramData, "Revit\Assemblies\ExcitechDOCS.Revision.dll")

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
                If Not loadfile.Contains("MSCORLIB") Then _sw.WriteLine("mscorlib")
                If Not loadfile.Contains(dllPath.ToUpperInvariant) Then _sw.WriteLine(dllPath)
            End Using

        End If

        If _client Then
            Dim svrInstaller As String = IO.Path.Combine(_target, "InstallFiles\ServerApplication\ExcitechDOCS.Application.Installer.exe")
            If Not IO.File.Exists(svrInstaller) Then Throw New Exception("Unable to find Server Installer: ExcitechDOCS.Application.Installer.exe")
            Dim cmdLine As String = svrInstaller + " " + ChrW(34) + "ExcitechDOCS document Revision" + ChrW(34) + " " + ChrW(34) + "ExDOCS_Revision_History.mfappx" + ChrW(34) + " " + ChrW(34) + "0F2B45EE-0DDE-4DAF-90B9-5BEAFD91F462" + ChrW(34)


            ''this will run a specifc application as the user
            '' ImpersonationUtils.LaunchAsCurrentUser(cmdLine)

            ''this will let you work in the user context
            Using ImpersonationUtils.ImpersonateCurrentUser
                Try
                    Dim _proc As New Process
                    _proc.StartInfo.Verb = "runas" ''must runas otherwise the process will break out of our impersonated context and start as the system
                    _proc.StartInfo.Arguments = " " + ChrW(34) + "ExcitechDOCS document Revision" + ChrW(34) + " " + ChrW(34) + "ExDOCS_Revision_History.mfappx" + ChrW(34) + " " + ChrW(34) + "0F2B45EE-0DDE-4DAF-90B9-5BEAFD91F462" + ChrW(34)
                    _proc.StartInfo.FileName = svrInstaller
                    _proc.StartInfo.ErrorDialog = True
                    _proc.Start()
                    _proc.WaitForExit()
                Catch ex As Exception
                    Throw New Exception("Unable to install server components " + vbCrLf + ex.ToString)
                End Try
            End Using

            ''show the client/install application
            'ExcitechDOCS.Application.Installer.exe "Document Revision History" "ExDOCS_Revision_History.mfappx" "0F2B45EE-0DDE-4DAF-90B9-5BEAFD91F462"
        End If

    End Sub

End Class
