Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Security.Principal

Module ImpersonationUtils
    Private Const SW_SHOW As Integer = 5
    Private Const TOKEN_QUERY As Integer = &H8
    Private Const TOKEN_DUPLICATE As Integer = &H2
    Private Const TOKEN_ASSIGN_PRIMARY As Integer = &H1
    Private Const STARTF_USESHOWWINDOW As Integer = &H1
    Private Const STARTF_FORCEONFEEDBACK As Integer = &H40
    Private Const CREATE_UNICODE_ENVIRONMENT As Integer = &H400
    Private Const TOKEN_IMPERSONATE As Integer = &H4
    Private Const TOKEN_QUERY_SOURCE As Integer = &H10
    Private Const TOKEN_ADJUST_PRIVILEGES As Integer = &H20
    Private Const TOKEN_ADJUST_GROUPS As Integer = &H40
    Private Const TOKEN_ADJUST_DEFAULT As Integer = &H80
    Private Const TOKEN_ADJUST_SESSIONID As Integer = &H100
    Private Const STANDARD_RIGHTS_REQUIRED As Integer = &HF0000
    Private Const TOKEN_ALL_ACCESS As Integer = STANDARD_RIGHTS_REQUIRED Or TOKEN_ASSIGN_PRIMARY Or TOKEN_DUPLICATE Or TOKEN_IMPERSONATE Or TOKEN_QUERY Or TOKEN_QUERY_SOURCE Or TOKEN_ADJUST_PRIVILEGES Or TOKEN_ADJUST_GROUPS Or TOKEN_ADJUST_DEFAULT Or TOKEN_ADJUST_SESSIONID

    <StructLayout(LayoutKind.Sequential)>
    Private Structure PROCESS_INFORMATION
        Public hProcess As IntPtr
        Public hThread As IntPtr
        Public dwProcessId As Integer
        Public dwThreadId As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure SECURITY_ATTRIBUTES
        Public nLength As Integer
        Public lpSecurityDescriptor As IntPtr
        Public bInheritHandle As Boolean
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure STARTUPINFO
        Public cb As Integer
        Public lpReserved As String
        Public lpDesktop As String
        Public lpTitle As String
        Public dwX As Integer
        Public dwY As Integer
        Public dwXSize As Integer
        Public dwYSize As Integer
        Public dwXCountChars As Integer
        Public dwYCountChars As Integer
        Public dwFillAttribute As Integer
        Public dwFlags As Integer
        Public wShowWindow As Short
        Public cbReserved2 As Short
        Public lpReserved2 As IntPtr
        Public hStdInput As IntPtr
        Public hStdOutput As IntPtr
        Public hStdError As IntPtr
    End Structure

    Private Enum SECURITY_IMPERSONATION_LEVEL
        SecurityAnonymous
        SecurityIdentification
        SecurityImpersonation
        SecurityDelegation
    End Enum

    Private Enum TOKEN_TYPE
        TokenPrimary = 1
        TokenImpersonation
    End Enum

    <DllImport("advapi32.dll", SetLastError:=True)>
    Private Function CreateProcessAsUser(ByVal hToken As IntPtr, ByVal lpApplicationName As String, ByVal lpCommandLine As String, ByRef lpProcessAttributes As SECURITY_ATTRIBUTES, ByRef lpThreadAttributes As SECURITY_ATTRIBUTES, ByVal bInheritHandles As Boolean, ByVal dwCreationFlags As Integer, ByVal lpEnvironment As IntPtr, ByVal lpCurrentDirectory As String, ByRef lpStartupInfo As STARTUPINFO, <Out> ByRef lpProcessInformation As PROCESS_INFORMATION) As Boolean
    End Function
    <DllImport("advapi32.dll", SetLastError:=True)>
    Private Function DuplicateTokenEx(ByVal hExistingToken As IntPtr, ByVal dwDesiredAccess As Integer, ByRef lpThreadAttributes As SECURITY_ATTRIBUTES, ByVal ImpersonationLevel As Integer, ByVal dwTokenType As Integer, ByRef phNewToken As IntPtr) As Boolean
    End Function

    <DllImport("advapi32.dll", SetLastError:=True)>
    Private Function OpenProcessToken(ByVal ProcessHandle As IntPtr, ByVal DesiredAccess As Integer, ByRef TokenHandle As IntPtr) As Boolean
    End Function

    <DllImport("userenv.dll", SetLastError:=True)>
    Private Function CreateEnvironmentBlock(ByRef lpEnvironment As IntPtr, ByVal hToken As IntPtr, ByVal bInherit As Boolean) As Boolean
    End Function

    <DllImport("userenv.dll", SetLastError:=True)>
    Private Function DestroyEnvironmentBlock(ByVal lpEnvironment As IntPtr) As Boolean
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Function CloseHandle(ByVal hObject As IntPtr) As Boolean
    End Function

    Private Sub LaunchProcessAsUser(ByVal cmdLine As String, ByVal token As IntPtr, ByVal envBlock As IntPtr, ByVal sessionId As Integer)
        Dim pi = New PROCESS_INFORMATION()
        Dim saProcess = New SECURITY_ATTRIBUTES()
        Dim saThread = New SECURITY_ATTRIBUTES()
        saProcess.nLength = Marshal.SizeOf(saProcess)
        saThread.nLength = Marshal.SizeOf(saThread)
        Dim si = New STARTUPINFO()
        si.cb = Marshal.SizeOf(si)
        si.lpDesktop = "WinSta0\Default"
        si.dwFlags = STARTF_USESHOWWINDOW Or STARTF_FORCEONFEEDBACK
        si.wShowWindow = SW_SHOW

        If Not CreateProcessAsUser(token, Nothing, cmdLine, saProcess, saThread, False, CREATE_UNICODE_ENVIRONMENT, envBlock, Nothing, si, pi) Then
            Throw New Win32Exception(Marshal.GetLastWin32Error(), "CreateProcessAsUser failed")
        End If
    End Sub

    Private Function Impersonate(ByVal token As IntPtr) As IDisposable
        Dim identity = New WindowsIdentity(token)
        Return identity.Impersonate()
    End Function

    Private Function GetPrimaryToken(ByVal process As Process) As IntPtr
        Dim token = IntPtr.Zero
        Dim primaryToken = IntPtr.Zero

        If OpenProcessToken(process.Handle, TOKEN_DUPLICATE, token) Then
            Dim sa = New SECURITY_ATTRIBUTES()
            sa.nLength = Marshal.SizeOf(sa)

            If Not DuplicateTokenEx(token, TOKEN_ALL_ACCESS, sa, CInt(SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation), CInt(TOKEN_TYPE.TokenPrimary), primaryToken) Then
                Throw New Win32Exception(Marshal.GetLastWin32Error(), "DuplicateTokenEx failed")
            End If

            CloseHandle(token)
        Else
            Throw New Win32Exception(Marshal.GetLastWin32Error(), "OpenProcessToken failed")
        End If

        Return primaryToken
    End Function

    Private Function GetEnvironmentBlock(ByVal token As IntPtr) As IntPtr
        Dim envBlock = IntPtr.Zero

        If Not CreateEnvironmentBlock(envBlock, token, False) Then
            Throw New Win32Exception(Marshal.GetLastWin32Error(), "CreateEnvironmentBlock failed")
        End If

        Return envBlock
    End Function

    Sub LaunchAsCurrentUser(ByVal cmdLine As String)
        Dim process = System.Diagnostics.Process.GetProcessesByName("explorer").FirstOrDefault()

        If process IsNot Nothing Then
            Dim token = GetPrimaryToken(process)

            If token <> IntPtr.Zero Then
                Dim envBlock = GetEnvironmentBlock(token)

                If envBlock <> IntPtr.Zero Then
                    LaunchProcessAsUser(cmdLine, token, envBlock, process.SessionId)

                    If Not DestroyEnvironmentBlock(envBlock) Then
                        Throw New Win32Exception(Marshal.GetLastWin32Error(), "DestroyEnvironmentBlock failed")
                    End If
                End If

                CloseHandle(token)
            End If
        End If
    End Sub

    Function ImpersonateCurrentUser() As IDisposable
        Dim process = System.Diagnostics.Process.GetProcessesByName("explorer").FirstOrDefault()

        If process IsNot Nothing Then
            Dim token = GetPrimaryToken(process)

            If token <> IntPtr.Zero Then
                Return Impersonate(token)
            End If
        End If

        Throw New Exception("Could not find explorer.exe")
    End Function
End Module
