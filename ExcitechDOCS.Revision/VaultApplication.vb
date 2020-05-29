Imports MFiles.VAF
Imports MFiles.VAF.Common
Imports MFiles.VAF.Configuration
Imports MFilesAPI

Public Class VaultApplication
    Inherits VaultApplicationBase

#Region " Constructor "
    Public Sub New()
        Try
            Dim licDecoder As New LicenseDecoder(LicenseDecoder.EncMode.TwoKey)

            ' This Is from the key file (MainKey.PublicXml).
            licDecoder.MainKey = "<RSAKeyValue><Modulus>vlZb42AyJpzUMFjN295gUMNT/hoo2/I8WkCmx/ujBFJw35+xO8vReGYBMlzjyRNFzgN7Ghbzqsnl9OfxB9Uvta8UDkxjcLEfblCsnHKpQDMcnBxB4DTczIpAUOYbYuy4/MQyC5GrCH2/PeFE99HRTH6yHH7jNahHUw6PNE2pmg0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"

            ' This Is from the key file (SecondKey.SecretXml)
            licDecoder.AltKey = "<RSAKeyValue><Modulus>vlq5h/JFBH39OxquCWp0Brnm6WQMtqHWEQcw9x34+p6WzrvaUKtkCUb+RmY+lhCgcqN1lRmdTs9oZdUaYsVvaPIhuhh5v3YePtdBa/xvv06yuXo2zhRGeaodE4RMN1WkA9ZAUs9YLDQUn/uBm3ZVcmMKHENEvJuILQMz0snPuK0=</Modulus><Exponent>AQAB</Exponent><P>/L41pq6bS3ku9N6iUwXS0k4dMz5W5xPcsCoF6B/gsE51AIqEU+UsojI7DTGAFHq5l+U1JXOpZ0FgargllI+3Xw==</P><Q>wM6ygSaGluse89L8KsXWt9jBW1aQ0Z82uTTWRAdA+ZZm2MfjrkXHl5DNmikRu7Ynbu5G2ypYPPnesAxBIbtHcw==</Q><DP>LmqwZ8BBfQbwfMA2h5DWOxFlg3e7dgzLxv6wvwS7uyVtj3/g9ZdtLwySk8W3hAtV8nOB4zLutavoDTFslXAfeQ==</DP><DQ>Wt5PpKyqi+AeA13xeJsrGhRu9IQ01oaJ/PmY7hDZH4gxyoNSm+TJL3aQX9JxSB2OMircfBhV488Dk8cCv0oLXw==</DQ><InverseQ>bFj/g3A0KMfRiK0szYKMzJRBPB+nIB6t5sRHQ7UjgSm+Y2XQPztnowFBVUG3aZ0YNrtUIxhOvGxptFhJzEhQYA==</InverseQ><D>ZwVY9h+DhOve+nb1C/mGNAG23EeerdUmsu6ObJ/XGWRtQBPhEtm/eVnn0hgR9UuoWoLm5zwGrBmKadqMvjoWkeeeR4USwomqD6A9Sb0pdvJ5spDmfXLAco3FpNRatcDO4wh7hyFmQK6EK/+KZSl7DwS2N8P60qN3CidK7lYd9i0=</D></RSAKeyValue>"

            License = New LicenseManagerBase(Of LicenseContentBase)(licDecoder)
        Catch ex As Exception

        End Try
    End Sub
#End Region

#Region " Extension Methods "

    Public Enum SortBy
        RevisionID
        CheckedBy
        CheckedDate
        ApproveBy
        ApproveDate
    End Enum

    Public Function AddDocumentRevision(Revision As DocumentRevision, jsonStr As String) As String
        Dim _revList As New RevisionList
        _revList = RevisionList.DeSeraliseRevisionList(jsonStr)

        _revList.Add(Revision)
        jsonStr = ""
        jsonStr = RevisionList.SeraliseRevisionList(_revList)

        Return jsonStr
    End Function

    'Public Function GetRevisionDetails(jsonStr As String, RevisionID As String, count As Integer, includeInternalRev As Boolean, wasShared As Boolean) As RevisionList

    '    Dim _revList As New RevisionList
    '    _revList = RevisionList.DeSeraliseRevisionList(jsonStr)
    '    Dim _mostRecent As New RevisionList

    '    If count <= -1 Then
    '        _mostRecent.AddRange(_revList.GetAll(RevisionID, includeInternalRev))
    '    Else
    '        _mostRecent.AddRange(_revList.GetMostRecent(RevisionID, count, includeInternalRev, wasShared))
    '        Dim _pad As Integer = count - _mostRecent.Count
    '        For _p As Integer = 0 To _pad - 1
    '            _mostRecent.Add(Nothing)
    '        Next
    '    End If

    '    Return _mostRecent
    'End Function

    Public Function GetRevisionDetails(jsonStr As String, RevisionID As String, includeInternalRev As Boolean, wasShared As Boolean) As RevisionList

        Dim _revList As New RevisionList
        _revList = RevisionList.DeSeraliseRevisionList(jsonStr)
        Dim _mostRecent As New RevisionList

        'If count <= -1 Then
        _mostRecent.AddRange(_revList.GetAll(RevisionID, includeInternalRev, wasShared))
        'Else
        '    _mostRecent.AddRange(_revList.GetMostRecent(RevisionID, count, includeInternalRev, wasShared))
        '    Dim _pad As Integer = count - _mostRecent.Count
        '    For _p As Integer = 0 To _pad - 1
        '        _mostRecent.Add(Nothing)
        '    Next
        'End If

        Return _mostRecent
    End Function

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
        License.Evaluate(vaultPersistent, False)

        ' Output the license status.
        Select Case License.LicenseStatus
            Case MFApplicationLicenseStatus.MFApplicationLicenseStatusTrial
                EventLog.WriteEntry("ExcitechDOCS Document Revision", "Extranet Application is running in a trial mode", EventLogEntryType.Information)
            Case MFApplicationLicenseStatus.MFApplicationLicenseStatusValid
                EventLog.WriteEntry("ExcitechDOCS Document Revision", "Extranet Application is licensed", EventLogEntryType.Information)
            Case Else
                EventLog.WriteEntry("ExcitechDOCS Document Revision", $"Extranet Application license is in an unexpected state: {License.LicenseStatus}.", EventLogEntryType.Error)
        End Select

        MyBase.StartOperations(vaultPersistent)
    End Sub
#End Region

End Class

