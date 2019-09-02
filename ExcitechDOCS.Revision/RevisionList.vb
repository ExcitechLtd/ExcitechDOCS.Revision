Public Class RevisionList
    Inherits List(Of DocumentRevision)

    Public Function GetMostRecent(RevisionID As String, Count As Integer, includeInternalRev As Boolean) As List(Of DocumentRevision)

        Return MyBase.Where(Function(_dr)
                                If includeInternalRev Then
                                    Return _dr.RevisionID.StartsWith(RevisionID)
                                Else
                                    Return _dr.RevisionID.StartsWith(RevisionID) And Not _dr.RevisionID.Contains(".")
                                End If
                            End Function).OrderBy(Function(_dr)
                                                      Return _dr.ApproveDate
                                                  End Function).Take(Count).ToList

    End Function

    Public Function GetAll(RevisionID As String, includeInternalRev As Boolean) As List(Of DocumentRevision)

        Return MyBase.Where(Function(_dr)
                                If includeInternalRev Then
                                    Return _dr.RevisionID.StartsWith(RevisionID)
                                Else
                                    Return _dr.RevisionID.StartsWith(RevisionID) And Not _dr.RevisionID.Contains(".")
                                End If
                            End Function).OrderBy(Function(_dr)
                                                      Return _dr.ApproveDate
                                                  End Function).ToList

    End Function

    Public Function RevisionItem(itemIndex As Integer) As DocumentRevision
        Return MyBase.Item(itemIndex)
    End Function

    Public Function Count() As Integer
        Return MyBase.Count
    End Function

    Public Shared Function SeraliseRevisionList(RevisionList As RevisionList) As String
        Dim _jsonStr As String = Newtonsoft.Json.JsonConvert.SerializeObject(RevisionList)
        Return _jsonStr
    End Function

    Public Shared Function DeSeraliseRevisionList(jsonString As String) As RevisionList
        Dim _revList As New RevisionList
        _revList = Newtonsoft.Json.JsonConvert.DeserializeObject(Of RevisionList)(jsonString)

        If _revList Is Nothing Then _revList = New RevisionList
        Return _revList
    End Function

    Public Overloads Sub Add(Revision As DocumentRevision)
        Dim _index As Integer = -1
        _index = MyBase.FindIndex(Function(rv)
                                      If Revision Is Nothing Then Return False

                                      Return rv.RevisionID = Revision.RevisionID
                                  End Function)

        If _index > -1 Then Exit Sub

        MyBase.Add(Revision)
    End Sub

End Class