Public Class DocumentRevision

    Public Property RevisionID As String
    Public Property Amendment As String
    Public Property CheckedBy As String
    Public Property CheckedDate As DateTime
    Public Property ApproveBy As String
    Public Property ApproveDate As DateTime
    Public Property WasShared As Boolean

    Public ReadOnly Property MajorRevisionID As String
        Get
            If RevisionID.Contains(".") Then
                Return RevisionID.Split(".")(0)
            Else
                Return RevisionID
            End If
        End Get
    End Property

    Public ReadOnly Property InternalRevisionID As String
        Get
            If RevisionID.Contains(".") Then
                Return RevisionID.Split(".")(1)
            Else
                Return ""
            End If
        End Get
    End Property

    Public Function GetAsString() As String
        Return RevisionID + "|" + Amendment + "|" + CheckedBy + "|" + CheckedDate.ToString + "|" + ApproveBy + "|" + ApproveDate.ToString + "|" + WasShared
    End Function
End Class

