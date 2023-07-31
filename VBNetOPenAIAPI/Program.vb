Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports System.IO

Public Class Program
    Public Shared Sub Main()

        LoadEnvironmentVariablesAsync().Wait() ' Wait for the LoadEnvironmentVariablesAsync task to complete synchronously
        Task.Run(AddressOf MainAsync).Wait() ' Wait for the MainAsync task to complete synchronously
    End Sub

    Public Shared Async Function LoadEnvironmentVariablesAsync() As Task
        Dim envFile As String = ".env"
        If File.Exists(envFile) Then
            Dim lines As String() = Await File.ReadAllLinesAsync(envFile)
            For Each line As String In lines
                Dim parts As String() = line.Split("="c)
                If parts.Length = 2 Then
                    Dim key As String = parts(0).Trim()
                    Dim value As String = parts(1).Trim()
                    Environment.SetEnvironmentVariable(key, value)
                End If
            Next
        End If
    End Function

    Public Shared Async Function MainAsync() As Task
        Dim openaiApiKey As String = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
        If String.IsNullOrEmpty(openaiApiKey) Then
            Console.WriteLine("Please set the OPENAI_API_KEY environment variable in the .env file.")
            Return
        End If

        Dim apiUrl As String = "https://api.openai.com/v1/chat/completions"



        Dim requestBody As String = "{""model"": ""gpt-3.5-turbo"", ""messages"": [{""role"": ""user"", ""content"": ""Hello This is a Test""}], ""temperature"": 0.7,""max_tokens"": 50 }"

        Using httpClient As New HttpClient()
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " & openaiApiKey)
            Dim content As New StringContent(requestBody, Encoding.UTF8, "application/json")

            Dim response As HttpResponseMessage = Await httpClient.PostAsync(apiUrl, content)
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()

            Console.WriteLine("Waiting for the output" & vbCrLf)

            ' Process the response content as needed
            Console.WriteLine("Here is the output" & vbCrLf)
            Console.WriteLine(responseContent)
        End Using
    End Function
End Class






