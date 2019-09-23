Imports MFiles.VAF
Imports MFiles.VAF.Common
Imports MFilesAPI

Public Class VaultApplication
    Inherits VaultApplicationBase

#Region " Constructor "

#End Region

#Region " Extension Methods "

    Public Function AddDocumentRevision(Revision As DocumentRevision, jsonStr As String) As String
        Dim _revList As New RevisionList
        _revList = RevisionList.DeSeraliseRevisionList(jsonStr)

        _revList.Add(Revision)
        jsonStr = ""
        jsonStr = RevisionList.SeraliseRevisionList(_revList)

        Return jsonStr
    End Function

    Public Function GetRevisionDetails(jsonStr As String, RevisionID As String, count As Integer, includeInternalRev As Boolean, wasShared As Boolean) As RevisionList

        Dim _revList As New RevisionList
        _revList = RevisionList.DeSeraliseRevisionList(jsonStr)
        Dim _mostRecent As New RevisionList

        If count <= -1 Then
            _mostRecent.AddRange(_revList.GetAll(RevisionID, includeInternalRev))
        Else
            _mostRecent.AddRange(_revList.GetMostRecent(RevisionID, count, includeInternalRev, wasShared))
            Dim _pad As Integer = count - _mostRecent.Count
            For _p As Integer = 0 To _pad - 1
                _mostRecent.Add(Nothing)
            Next
        End If

        Return _mostRecent
    End Function


    'Public Sub SaveDocumentRevision(objVer As ObjVer, Revision As DocumentRevision)
    '    Dim _revList As RevisionList = GetObjVerRevisions(objVer)
    '    _revList.Add(Revision)
    '    Dim _jsonStr As String = RevisionList.SeraliseRevisionList(_revList)
    '    ''for now to test just save to a file
    '    Using _sw As New IO.StreamWriter("c:\temp\document.revision", False)
    '        _sw.WriteLine(_jsonStr)
    '    End Using
    'End Sub

    'Public Function GetObjVerRevisions(objver As ObjVer) As RevisionList
    '    Dim _revList As New RevisionList
    '    Dim _jsonStr As String = ""

    '    If IO.File.Exists("c:\temp\document.revision") Then _jsonStr = IO.File.ReadAllText("c:\temp\document.revision")
    '    _revList = RevisionList.DeSeraliseRevisionList(_jsonStr)

    '    ''load the seralised data and return the revision list
    '    If _revList Is Nothing Then _revList = New RevisionList
    '    Return _revList
    'End Function

    'Public Function GetMostRecentRevisionDetails(objver As ObjVer, RevisionID As String, count As Integer, includeInternalRev As Boolean) As RevisionList
    '    Dim _revList As RevisionList = GetObjVerRevisions(objver)
    '    Dim _mostRecent As New RevisionList
    '    _mostRecent.AddRange(_revList.GetMostRecent(RevisionID, count, includeInternalRev))

    '    Dim _pad As Integer = count - _mostRecent.Count
    '    For _p As Integer = 0 To _pad - 1
    '        _mostRecent.Add(Nothing)
    '    Next

    '    Return _mostRecent
    'End Function

    Public Function GetRevisionObject() As DocumentRevision
        Return New DocumentRevision
    End Function

#End Region

#Region " Init "
    Public Overrides Sub Initialize(vaultSrc As Vault)
        MyBase.Initialize(vaultSrc)
    End Sub

    Public Overrides Sub Uninitialize(vault As Vault)
        MyBase.Uninitialize(vault)
    End Sub

    Protected Overrides Sub StartApplication()
        MyBase.StartApplication()
    End Sub

    Public Overrides Sub StartOperations(vaultPersistent As Vault)
        MyBase.StartOperations(vaultPersistent)
    End Sub
#End Region

End Class

