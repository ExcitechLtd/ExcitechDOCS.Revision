Public Class DocumentRevision

    Public Property RevisionID As String
    Public Property Amendment As String
    Public Property CheckedBy As String
    Public Property CheckedDate As DateTime
    Public Property ApproveBy As String
    Public Property ApproveDate As DateTime


    Public Function GetAsString() As String
        Return RevisionID + "|" + Amendment + "|" + CheckedBy + "|" + CheckedDate.ToString + "|" + ApproveBy + "|" + ApproveDate.ToString
    End Function
End Class

