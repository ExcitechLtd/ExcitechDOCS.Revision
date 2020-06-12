Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim rev As New ExcitechDOCS.Revision.VaultApplication

        Dim jsonStr As String
        Using _sr As New IO.StreamReader("c:\temp\jsonstr.txt")
            jsonStr = _sr.ReadToEnd
        End Using

        Dim _revList = rev.GetRevisionDetails(jsonStr, "P", True, False)

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim revList As New List(Of String)
        revList.Add("C01")
        revList.Add("C02")
        revList.Add("C03")
        revList.Add("C04")
        revList.Add("C05")
        revList.Add("C06")
        revList.Add("C08")
        revList.Add("C07")
        revList.Add("C09")
        revList.Add("C10")
        revList.Add("C11")
        revList.Add("C12")
        revList.Add("C13")
        revList.Add("C12")


        revList.Sort(Function(x, y)
                         Return String.Compare(y, x)
                     End Function)

        For Each _str In revList
            Debug.WriteLine(_str)
        Next

    End Sub
End Class
