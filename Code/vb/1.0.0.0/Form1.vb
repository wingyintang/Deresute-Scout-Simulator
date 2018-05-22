Imports System.Runtime.CompilerServices
Imports System.Drawing.Drawing2D

Module GraphicsExtensions

    <Extension()>
    Public Sub DrawStringBorder(g As Graphics, s As String, font As Font, brushi As Brush, brusho As Brush, point As PointF, border As Integer)
        For i = -border To border
            For j = -border To border
                g.DrawString(s, font, brusho, point + New Point(i, j))
            Next
        Next
        g.DrawString(s, font, brushi, point)
    End Sub

End Module
Public Class Form1
    Dim widths As Integer = 1280
    Dim heights As Integer = (widths * 9) \ 16
    Dim signbase As New Bitmap(widths, heights)
    Dim wholebase As New Bitmap(widths, heights)
    Dim modified As New Bitmap(widths, heights)
    Dim completed As New Bitmap(widths, heights)
    Dim smallsign As New Bitmap(widths, heights)
    Dim image As New Bitmap(widths, heights)
    Dim pensize As Double = 6 * widths / 1280 '2~6
    Dim border As Double = 12 * widths / 1280
    Dim addsimage As Bitmap = My.Resources.cute_frame
    Dim addsimages As Bitmap
    Dim des As String = ""
    Dim names As String = ""
    Dim clicked As Boolean = False
    Dim loaded As Boolean = False
    Dim lastpt As Point
    Dim element As Integer = 0 'Cute/Cool/Passion
    Dim lastframefinished As Boolean = True
    'Color Gradient
    '_______________
    '_Gradient Only_
    '_______________
    'cute: rgb(255, 138, 232) rgb(107, 233, 255)
    'cool: rgb(125, 215, 255) rgb(255, 229, 133)
    'passion: rgb(255, 210, 75) rgb(255, 138, 232)
    Function resizeimg(a As Bitmap) As Bitmap
        Dim b As New Bitmap(widths, heights)
        Using g As Graphics = Graphics.FromImage(b)
            g.DrawImage(a, New Rectangle(New Point(0, 0), New Size(widths, heights)))
        End Using
        Return b
    End Function
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim b As New Bitmap(widths, heights)
        Using g As Graphics = Graphics.FromImage(b)
            g.Clear(Color.Black)
            g.DrawImage(My.Resources.orb, New Rectangle(New Point(0, 0), New Size(widths, heights)))
            g.DrawImage(My.Resources.flare, New Rectangle(New Point(0, 0), New Size(widths, heights)))
        End Using
        PictureBox1.BackgroundImage = b
        loaded = True
    End Sub
    Function cpt(a As Point) As Point
        Dim ratio As Double = widths / PictureBox1.Width
        Return New Point(a.X * ratio, a.Y * ratio)
    End Function
    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseDown
        clicked = True
        lastpt = e.Location
    End Sub
    Sub converts()
        Dim rect As New Rectangle(0, 0, modified.Width, modified.Height)
        Dim bmpData As System.Drawing.Imaging.BitmapData = modified.LockBits(rect, Drawing.Imaging.ImageLockMode.ReadWrite, modified.PixelFormat)
        Dim ptr As IntPtr = bmpData.Scan0
        Dim bytes As Integer = Math.Abs(bmpData.Stride) * modified.Height
        Dim rgbValues(bytes - 1) As Byte
        System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes)
        Dim pixelbyte As Integer = Math.Abs(bmpData.Stride) \ modified.Width 'should be 4

        Dim bmpData2 As System.Drawing.Imaging.BitmapData = wholebase.LockBits(rect, Drawing.Imaging.ImageLockMode.ReadWrite, wholebase.PixelFormat)
        Dim ptr2 As IntPtr = bmpData2.Scan0
        Dim rgbValues2(bytes - 1) As Byte
        System.Runtime.InteropServices.Marshal.Copy(ptr2, rgbValues2, 0, bytes)
        'Start
        For y = 0 To bmpData.Height - 1
            Dim r, g, b As Integer
            Select Case y
                Case 0 To bmpData.Height \ 3
                    r = {255, 125, 255}(element)
                    g = {138, 215, 210}(element)
                    b = {232, 255, 75}(element)
                Case bmpData.Height \ 3 + 1 To bmpData.Height * 2 \ 3
                    Dim p As Double = (y - (bmpData.Height \ 3 + 1)) / ((bmpData.Height * 2 \ 3) - (bmpData.Height \ 3 + 1))
                    r = CInt({255, 125, 255}(element) + {-148, 130, 0}(element) * p)
                    g = CInt({138, 215, 210}(element) + {95, 14, -72}(element) * p)
                    b = CInt({232, 255, 75}(element) + {23, -122, 157}(element) * p)
                Case bmpData.Height * 2 \ 3 + 1 To bmpData.Height - 1
                    r = {107, 255, 255}(element)
                    g = {233, 229, 138}(element)
                    b = {255, 133, 232}(element)
            End Select
            Dim baseoffset As Integer = y * bmpData.Width
            For x = 0 To bmpData.Width - 1
                If rgbValues2((baseoffset + x) * pixelbyte + 3) > 0 Then
                    'MsgBox(x & "," & y & ": " & rgbValues2((baseoffset + x) * pixelbyte + 3))
                    rgbValues((baseoffset + x) * pixelbyte + 3) = 255
                    rgbValues((baseoffset + x) * pixelbyte + 2) = r
                    rgbValues((baseoffset + x) * pixelbyte + 1) = g
                    rgbValues((baseoffset + x) * pixelbyte + 0) = b
                End If
            Next
        Next
        'End
        System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes)
        modified.UnlockBits(bmpData)
        System.Runtime.InteropServices.Marshal.Copy(rgbValues2, 0, ptr2, bytes)
        wholebase.UnlockBits(bmpData2)
    End Sub
    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        If clicked Then
            Dim newpt As Point = e.Location
            Using g As Graphics = Graphics.FromImage(signbase)
                g.DrawLine(New Pen(Brushes.White, pensize), cpt(lastpt), cpt(newpt))
                'Smooth
                g.FillPie(Brushes.White, New Rectangle(cpt(lastpt) - New Point(pensize / 2, pensize / 2), New Size(pensize, pensize)), 0, 360)
                g.FillPie(Brushes.White, New Rectangle(cpt(newpt) - New Point(pensize / 2, pensize / 2), New Size(pensize, pensize)), 0, 360)
            End Using
            Using g As Graphics = Graphics.FromImage(wholebase)
                g.DrawLine(New Pen(Brushes.White, pensize + border * 2), cpt(lastpt), cpt(newpt))
                'Smooth
                g.FillPie(Brushes.White, New Rectangle(cpt(lastpt) - New Point(border + pensize / 2, border + pensize / 2), New Size(pensize + border * 2, pensize + border * 2)), 0, 360)
                g.FillPie(Brushes.White, New Rectangle(cpt(newpt) - New Point(border + pensize / 2, border + pensize / 2), New Size(pensize + border * 2, pensize + border * 2)), 0, 360)
            End Using
            PictureBox1.Image = signbase
            lastpt = newpt
        End If
    End Sub

    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseUp
        converts()
        Using g As Graphics = Graphics.FromImage(completed)
            g.DrawImage(modified, New Rectangle(New Point(0, 0), New Size(widths, heights)))
            g.DrawImage(signbase, New Rectangle(New Point(0, 0), New Size(widths, heights)))
        End Using
        PictureBox1.Image = completed
        clicked = False
    End Sub

    Private Sub ElementChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged, RadioButton2.CheckedChanged, RadioButton3.CheckedChanged
        If loaded AndAlso sender.checked Then
            element = CInt(Strings.Mid(sender.name, 12)) - 1
            addsimage = {My.Resources.cute_frame, My.Resources.cool_frame, My.Resources.passion_frame}(element)
            addsimages = addsimage
            converts()
            Using g As Graphics = Graphics.FromImage(completed)
                g.DrawImage(modified, New Rectangle(New Point(0, 0), New Size(widths, heights)))
                g.DrawImage(signbase, New Rectangle(New Point(0, 0), New Size(widths, heights)))
            End Using
            PictureBox1.Image = completed
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            image = New Bitmap(widths, heights)
            Dim img As New Bitmap(OpenFileDialog1.FileName)
            Dim aspect As Double = img.Width / img.Height
            Using g As Graphics = Graphics.FromImage(image)
                Dim offset As Point
                If aspect > 16 / 9 Then 'Too Wide
                    offset = New Point((widths - heights * aspect) \ 2, 0)
                    g.DrawImage(img, New Rectangle(offset, New Size(heights * aspect, heights)))
                Else
                    offset = New Point(0, (heights - widths / aspect) \ 2)
                    g.DrawImage(img, New Rectangle(offset, New Size(widths, widths / aspect)))
                End If
            End Using
            'PictureBox1.Image = image
            Label1.Text = "Selected"
            Button2.Enabled = True
        End If
    End Sub

    Dim timestart As DateTime
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Using g As Graphics = Graphics.FromImage(smallsign)
            g.DrawImage(completed, New Rectangle(New Point(0, heights \ 2), New Size(widths \ 2, heights \ 2)))
        End Using
        'Finish addsimages
        addsimages = addsimage.Clone
        Dim color1 As Color = {Color.FromArgb(255, 26, 140), Color.FromArgb(33, 128, 222), Color.FromArgb(254, 119, 0)}(element)
        Dim color2 As Color = {Color.FromArgb(255, 122, 189), Color.FromArgb(89, 180, 222), Color.FromArgb(255, 170, 0)}(element)
        Using g As Graphics = Graphics.FromImage(addsimages)
            Dim s As SizeF = g.MeasureString("[" & des & "]", New Font("Meiryo", 42 * 0.75, Drawing.FontStyle.Bold))
            Dim lgb1 As New LinearGradientBrush(New Point(0, 0), New Point(0, s.Height), color1, color2)
            g.DrawStringBorder("[" & des & "]", New Font("Meiryo", 42 * 0.75, Drawing.FontStyle.Bold), lgb1, Brushes.White, New Point(320, 85) - New Point(10, s.Height * 0.6), 4) 'Left Align
            Dim s2 As SizeF = g.MeasureString(names, New Font("Meiryo", 90 * 0.75, Drawing.FontStyle.Bold))
            Dim lgb2 As New LinearGradientBrush(New Point(0, 0), New Point(0, s2.Height), color1, color2)
            g.DrawStringBorder(names, New Font("Meiryo", 90 * 0.75, Drawing.FontStyle.Bold), lgb2, Brushes.White, New Point(400, 190) - New Point(s2.Width / 2 - 10, s2.Height * 0.6), 4) 'Center Align
        End Using
        'Sound part
        My.Computer.Audio.Play(My.Resources.SSR, AudioPlayMode.Background)
        PictureBox1.Show()
        PictureBox2.Show()
        timestart = Now
        Timer1.Start()
    End Sub
    Function transparency(c As Bitmap, a As Double) As Bitmap
        Dim b As Bitmap = c.Clone
        Dim rect As New Rectangle(0, 0, b.Width, b.Height)
        Dim bmpData As System.Drawing.Imaging.BitmapData = b.LockBits(rect, Drawing.Imaging.ImageLockMode.ReadWrite, b.PixelFormat)
        Dim ptr As IntPtr = bmpData.Scan0
        Dim bytes As Integer = Math.Abs(bmpData.Stride) * b.Height
        Dim rgbValues(bytes - 1) As Byte
        Dim pixelbyte As Integer = Math.Abs(bmpData.Stride) \ modified.Width 'should be 4
        System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes)
        For i = 3 To bytes - 1 Step 4
            rgbValues(i) = CInt(rgbValues(i) * a)
        Next
        System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes)
        b.UnlockBits(bmpData)
        Return b
    End Function
    Function addimage(ori As Bitmap, newi As Bitmap, Optional mode As String = "source-over") As Bitmap
        Dim ori2 As Bitmap = ori.Clone
        Dim rect As New Rectangle(0, 0, ori2.Width, ori2.Height)
        Dim bmpData As System.Drawing.Imaging.BitmapData = ori2.LockBits(rect, Drawing.Imaging.ImageLockMode.ReadWrite, ori2.PixelFormat)
        Dim ptr As IntPtr = bmpData.Scan0
        Dim bytes As Integer = Math.Abs(bmpData.Stride) * ori2.Height
        Dim rgbValues(bytes - 1) As Byte
        System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes)
        Dim pixelbyte As Integer = Math.Abs(bmpData.Stride) \ ori2.Width 'should be 4
        Dim bmpData2 As System.Drawing.Imaging.BitmapData = newi.LockBits(rect, Drawing.Imaging.ImageLockMode.ReadWrite, newi.PixelFormat)
        Dim ptr2 As IntPtr = bmpData2.Scan0
        Dim rgbValues2(bytes - 1) As Byte
        Dim pixelbyte2 As Integer = Math.Abs(bmpData2.Stride) \ newi.Width 'should be 4
        System.Runtime.InteropServices.Marshal.Copy(ptr2, rgbValues2, 0, bytes)
        'Start
        For y = 0 To bmpData.Height - 1
            Dim baseoffset As Integer = y * bmpData.Width
            For x = 0 To bmpData.Width - 1
                Dim r1, g1, b1, a1 As Byte
                Dim r2, g2, b2, a2 As Byte
                r1 = rgbValues((baseoffset + x) * pixelbyte + 2)
                g1 = rgbValues((baseoffset + x) * pixelbyte + 1)
                b1 = rgbValues((baseoffset + x) * pixelbyte + 0)
                r2 = rgbValues2((baseoffset + x) * pixelbyte + 2)
                g2 = rgbValues2((baseoffset + x) * pixelbyte + 1)
                b2 = rgbValues2((baseoffset + x) * pixelbyte + 0)
                r2 = (CInt(r1) * (255 - a2) + CInt(r2) * a2) \ 255
                g2 = (CInt(g1) * (255 - a2) + CInt(g2) * a2) \ 255
                b2 = (CInt(b1) * (255 - a2) + CInt(b2) * a2) \ 255
                If pixelbyte = 4 Then
                    a1 = rgbValues((baseoffset + x) * pixelbyte + 3)
                Else
                    a1 = 255
                End If
                If pixelbyte2 = 4 Then
                    a2 = rgbValues2((baseoffset + x) * pixelbyte + 3)
                Else
                    a2 = 255
                End If
                Select Case mode
                    Case "source-atop" ' Support All
                        If a1 > 0 And a2 > 0 Then
                            rgbValues((baseoffset + x) * pixelbyte + 2) = r2
                            rgbValues((baseoffset + x) * pixelbyte + 1) = g2
                            rgbValues((baseoffset + x) * pixelbyte + 0) = b2
                        End If
                    Case "source-in"
                        If a1 > 0 Then
                            If a2 > 0 Then
                                rgbValues((baseoffset + x) * pixelbyte + 2) = r2
                                rgbValues((baseoffset + x) * pixelbyte + 1) = g2
                                rgbValues((baseoffset + x) * pixelbyte + 0) = b2
                            Else
                                If pixelbyte = 4 Then
                                    rgbValues((baseoffset + x) * pixelbyte + 3) = 0
                                Else
                                    Throw New ArgumentException("No such method")
                                End If
                            End If
                        End If
                    Case "source-out"
                        If a1 > 0 Then
                            If pixelbyte = 4 Then
                                rgbValues((baseoffset + x) * pixelbyte + 3) = 0
                            Else
                                Throw New ArgumentException("No such method")
                            End If
                        ElseIf a2 > 0 Then
                            rgbValues((baseoffset + x) * pixelbyte + 2) = r2
                            rgbValues((baseoffset + x) * pixelbyte + 1) = g2
                            rgbValues((baseoffset + x) * pixelbyte + 0) = b2
                        End If
                    Case "source-over"
                        If a2 > 0 Then
                            rgbValues((baseoffset + x) * pixelbyte + 2) = r2
                            rgbValues((baseoffset + x) * pixelbyte + 1) = g2
                            rgbValues((baseoffset + x) * pixelbyte + 0) = b2
                        End If
                    Case "destination-atop"
                        If a1 > 0 Then
                            If a2 = 0 Then
                                If pixelbyte = 4 Then
                                    rgbValues((baseoffset + x) * pixelbyte + 3) = 0
                                Else
                                    Throw New ArgumentException("No such method")
                                End If
                            End If
                        ElseIf a2 > 0 Then
                            rgbValues((baseoffset + x) * pixelbyte + 2) = r2
                            rgbValues((baseoffset + x) * pixelbyte + 1) = g2
                            rgbValues((baseoffset + x) * pixelbyte + 0) = b2
                        End If
                    Case "destination-in"
                        If a1 > 0 Then
                            If a2 = 0 Then
                                If pixelbyte = 4 Then
                                    rgbValues((baseoffset + x) * pixelbyte + 3) = 0
                                Else
                                    Throw New ArgumentException("No such method")
                                End If
                            End If
                        End If
                    Case "destination-out"
                        If a1 > 0 Then
                            If a2 > 0 Then
                                If pixelbyte = 4 Then
                                    rgbValues((baseoffset + x) * pixelbyte + 3) = 0
                                Else
                                    Throw New ArgumentException("No such method")
                                End If
                            End If
                        End If
                    Case "destination-over"
                        If a2 > 0 Then
                            If a1 = 0 Then
                                rgbValues((baseoffset + x) * pixelbyte + 2) = r2
                                rgbValues((baseoffset + x) * pixelbyte + 1) = g2
                                rgbValues((baseoffset + x) * pixelbyte + 0) = b2
                            End If
                        End If
                    Case "lighter"
                        If a2 > 0 Then
                            If a1 > 0 Then
                                rgbValues((baseoffset + x) * pixelbyte + 2) = Math.Min(CInt(r1) + r2, 255)
                                rgbValues((baseoffset + x) * pixelbyte + 1) = Math.Min(CInt(g1) + g2, 255)
                                rgbValues((baseoffset + x) * pixelbyte + 0) = Math.Min(CInt(b1) + b2, 255)
                            Else
                                rgbValues((baseoffset + x) * pixelbyte + 2) = r2
                                rgbValues((baseoffset + x) * pixelbyte + 1) = g2
                                rgbValues((baseoffset + x) * pixelbyte + 0) = b2
                            End If
                        End If
                    Case "copy"
                        If a2 > 0 Then
                            rgbValues((baseoffset + x) * pixelbyte + 2) = r2
                            rgbValues((baseoffset + x) * pixelbyte + 1) = g2
                            rgbValues((baseoffset + x) * pixelbyte + 0) = b2
                        Else
                            If pixelbyte = 4 Then
                                rgbValues((baseoffset + x) * pixelbyte + 3) = 0
                            Else
                                Throw New ArgumentException("No such method")
                            End If
                        End If
                    Case "xor"
                        If a2 > 0 Then
                            If a1 > 0 Then
                                If pixelbyte = 4 Then
                                    rgbValues((baseoffset + x) * pixelbyte + 3) = 0
                                Else
                                    Throw New ArgumentException("No such method")
                                End If
                            Else
                                rgbValues((baseoffset + x) * pixelbyte + 2) = r2
                                rgbValues((baseoffset + x) * pixelbyte + 1) = g2
                                rgbValues((baseoffset + x) * pixelbyte + 0) = b2
                            End If
                        End If
                    Case "difference"
                        If a2 > 0 Then
                            If a1 > 0 Then
                                rgbValues((baseoffset + x) * pixelbyte + 2) = Math.Min(Math.Abs(CInt(r1) - r2), 255)
                                rgbValues((baseoffset + x) * pixelbyte + 1) = Math.Min(Math.Abs(CInt(g1) - g2), 255)
                                rgbValues((baseoffset + x) * pixelbyte + 0) = Math.Min(Math.Abs(CInt(b1) - b2), 255)
                            Else
                                rgbValues((baseoffset + x) * pixelbyte + 2) = r2
                                rgbValues((baseoffset + x) * pixelbyte + 1) = g2
                                rgbValues((baseoffset + x) * pixelbyte + 0) = b2
                            End If
                        End If
                End Select
            Next
        Next
        'End
        System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes)
        ori2.UnlockBits(bmpData)
        System.Runtime.InteropServices.Marshal.Copy(rgbValues2, 0, ptr2, bytes)
        newi.UnlockBits(bmpData2)
        Return ori2
    End Function
    Function rotatept(targetpt As Point, origin As Point, rotatedeg As Double)
        If targetpt = origin Then
            Return origin
        Else
            Dim distance As Double = Math.Sqrt((origin.X - targetpt.X) ^ 2 + (origin.Y - targetpt.Y) ^ 2)
            Dim angle As Double = Math.Atan2(origin.Y - targetpt.Y, targetpt.X - origin.X) + rotatedeg * Math.PI / 180
            Return origin + New Point(distance * Math.Cos(angle), -distance * Math.Sin(angle))
        End If
    End Function
    Sub drawrotatedimage(ByRef g As Graphics, b As Bitmap, r As Rectangle, o As Point, d As Double)
        Dim pt1 As Point = r.Location
        Dim pt2 As Point = rotatept(pt1 + New Point(r.Width, 0), o, d)
        Dim pt3 As Point = rotatept(pt1 + New Point(0, r.Height), o, d)
        g.DrawImage(b, {pt1, pt2, pt3})
    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If lastframefinished Then
            lastframefinished = False
            Dim tp As Integer = (Now - timestart).TotalMilliseconds
            Dim f As Integer = Math.Min(CInt(Int(tp * 0.03)), 365) + 1
            Dim base As Bitmap = resizeimg(New Bitmap(Application.StartupPath & "\files\scene" & f.ToString.PadLeft(5, "0") & ".png"))
            If tp >= 4500 And tp <= 7100 Then
                If tp <= 5200 Then
                    base = addimage(base, transparency(completed, (tp - 4500) / 700), "lighter")
                ElseIf tp >= 6400 Then
                    base = addimage(base, transparency(completed, (7100 - tp) / 700), "lighter")
                Else
                    base = addimage(base, completed, "lighter")
                End If
            End If
            If tp >= 7000 Then
                base = addimage(base, image, "lighter")
            End If
            If tp >= 10000 Then
                If tp <= 11000 Then
                    base = addimage(base, transparency(smallsign, (tp - 10000) / 1000), "source-over")
                ElseIf tp >= 19000 Then
                    base = addimage(base, transparency(smallsign, (20000 - tp) / 1000), "source-over")
                Else
                    base = addimage(base, smallsign, "source-over")
                End If
            End If
            If tp >= 6500 And tp <= 7000 Then
                base = addimage(base, transparency(completed, (tp - 6500) / 500), "difference")
            End If
            If tp >= 10000 Then
                Using g As Graphics = Graphics.FromImage(base)
                    Dim ratio As Double
                    If tp >= 10500 Then
                        ratio = 0
                    Else
                        ratio = (10500 - tp) / 500
                    End If
                    Dim pt1 As Point = New Point(widths * 350 / 720, widths * 270 / 720) + New Point(widths * 300 * ratio / 720 * Math.Cos(Math.PI / 18), -widths * 300 * ratio / 720 * Math.Sin(Math.PI / 18))
                    drawrotatedimage(g, addsimages, New Rectangle(pt1, New Size(widths * 807 * 0.4 / 720, widths * 286 * 0.4 / 720)), pt1, 10)
                End Using
            End If
            'To 12200
            PictureBox2.Image = base
            If tp >= 12200 Then
                Timer1.Stop()
            End If
            GC.Collect()
            lastframefinished = True
        End If
    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        Timer1.Stop()
        My.Computer.Audio.Stop()
        PictureBox2.Hide()
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        des = TextBox1.Text
    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        names = TextBox2.Text
    End Sub
    Sub loadsign(c As Bitmap)
        Dim b As Bitmap = c.Clone
        signbase = New Bitmap(widths, heights)
        modified = New Bitmap(widths, heights)
        wholebase = New Bitmap(widths, heights)
        completed = New Bitmap(widths, heights)
        smallsign = New Bitmap(widths, heights)
        Using g As Graphics = Graphics.FromImage(signbase)
            Dim aspect As Double = b.Width / b.Height
            Dim offset As Point
            If aspect > 16 / 9 Then 'Too Wide
                offset = New Point(0, (heights - widths / aspect) \ 2)
                g.DrawImage(b, New Rectangle(offset, New Size(widths, widths / aspect)))
            Else
                offset = New Point((widths - heights * aspect) \ 2, 0)
                g.DrawImage(b, New Rectangle(offset, New Size(heights * aspect, heights)))
            End If
        End Using
        Dim rect As New Rectangle(0, 0, signbase.Width, signbase.Height)
        Dim bmpData As System.Drawing.Imaging.BitmapData = signbase.LockBits(rect, Drawing.Imaging.ImageLockMode.ReadWrite, signbase.PixelFormat)
        Dim ptr As IntPtr = bmpData.Scan0
        Dim bytes As Integer = Math.Abs(bmpData.Stride) * signbase.Height
        Dim rgbValues(bytes - 1) As Byte
        Dim pixelbyte As Integer = Math.Abs(bmpData.Stride) \ modified.Width 'should be 4
        System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes)
        For y = 0 To signbase.Height - 1
            For x = 0 To signbase.Width - 1
                If rgbValues((y * signbase.Width + x) * 4 + 3) > 128 Then
                    rgbValues((y * signbase.Width + x) * 4 + 3) = 255
                    rgbValues((y * signbase.Width + x) * 4 + 2) = 255
                    rgbValues((y * signbase.Width + x) * 4 + 1) = 255
                    rgbValues((y * signbase.Width + x) * 4 + 0) = 255
                Else
                    rgbValues((y * signbase.Width + x) * 4 + 3) = 0
                End If
            Next
        Next
        System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes)
        signbase.UnlockBits(bmpData)
        wholebase = signbase.Clone
        Dim wholebase2 = signbase.Clone
        'Edge Detection border times
        Dim rect2 As New Rectangle(0, 0, wholebase.Width, wholebase.Height)
        Dim bmpData2 As System.Drawing.Imaging.BitmapData = wholebase.LockBits(rect2, Drawing.Imaging.ImageLockMode.ReadWrite, wholebase.PixelFormat)
        Dim ptr2 As IntPtr = bmpData2.Scan0
        Dim bytes2 As Integer = Math.Abs(bmpData2.Stride) * wholebase.Height
        Dim rgbValues2(bytes2 - 1) As Byte
        Dim rgbValues3() As Byte = rgbValues2.Clone
        Dim pixelbyte2 As Integer = Math.Abs(bmpData2.Stride) \ modified.Width 'should be 4
        System.Runtime.InteropServices.Marshal.Copy(ptr2, rgbValues2, 0, bytes2)
        For i = 0 To border - 1
            For y = 1 To wholebase.Height - 1
                For x = 0 To wholebase.Width - 1
                    Dim a1, a2 As Integer
                    a1 = rgbValues2((y * wholebase.Width + x) * 4 + 3)
                    a2 = rgbValues2(((y - 1) * wholebase.Width + x) * 4 + 3)
                    If a1 + a2 = 255 Then
                        If a1 = 255 Then
                            rgbValues3(((y - 1) * wholebase.Width + x) * 4 + 3) = 255
                            rgbValues3(((y - 1) * wholebase.Width + x) * 4 + 2) = 255
                            rgbValues3(((y - 1) * wholebase.Width + x) * 4 + 1) = 255
                            rgbValues3(((y - 1) * wholebase.Width + x) * 4 + 0) = 255
                        Else
                            rgbValues3((y * wholebase.Width + x) * 4 + 3) = 255
                            rgbValues3((y * wholebase.Width + x) * 4 + 2) = 255
                            rgbValues3((y * wholebase.Width + x) * 4 + 1) = 255
                            rgbValues3((y * wholebase.Width + x) * 4 + 0) = 255
                        End If
                    End If
                Next
            Next
            For y = 0 To wholebase.Height - 1
                For x = 1 To wholebase.Width - 1
                    Dim a1, a2 As Integer
                    a1 = rgbValues2((y * wholebase.Width + x) * 4 + 3)
                    a2 = rgbValues2((y * wholebase.Width + x - 1) * 4 + 3)
                    If a1 + a2 = 255 Then
                        If a1 = 255 Then
                            rgbValues3((y * wholebase.Width + x - 1) * 4 + 3) = 255
                            rgbValues3((y * wholebase.Width + x - 1) * 4 + 2) = 255
                            rgbValues3((y * wholebase.Width + x - 1) * 4 + 1) = 255
                            rgbValues3((y * wholebase.Width + x - 1) * 4 + 0) = 255
                        Else
                            rgbValues3((y * wholebase.Width + x) * 4 + 3) = 255
                            rgbValues3((y * wholebase.Width + x) * 4 + 2) = 255
                            rgbValues3((y * wholebase.Width + x) * 4 + 1) = 255
                            rgbValues3((y * wholebase.Width + x) * 4 + 0) = 255
                        End If
                    End If
                Next
            Next
            rgbValues2 = rgbValues3.Clone
        Next
        System.Runtime.InteropServices.Marshal.Copy(rgbValues2, 0, ptr2, bytes2)
        wholebase.UnlockBits(bmpData2)
        converts()
        Using g As Graphics = Graphics.FromImage(completed)
            g.DrawImage(modified, New Rectangle(New Point(0, 0), New Size(widths, heights)))
            g.DrawImage(signbase, New Rectangle(New Point(0, 0), New Size(widths, heights)))
        End Using
        PictureBox1.Image = completed
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If OpenFileDialog2.ShowDialog = Windows.Forms.DialogResult.OK Then
            loadsign(New Bitmap(OpenFileDialog2.FileName))
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim img As Bitmap = My.Resources.bg_shizuku
        Dim aspect As Double = img.Width / img.Height
        Using g As Graphics = Graphics.FromImage(image)
            Dim offset As Point
            If aspect > 16 / 9 Then 'Too Wide
                offset = New Point((widths - heights * aspect) \ 2, 0)
                g.DrawImage(img, New Rectangle(offset, New Size(heights * aspect, heights)))
            Else
                offset = New Point(0, (heights - widths / aspect) \ 2)
                g.DrawImage(img, New Rectangle(offset, New Size(widths, widths / aspect)))
            End If
        End Using
        loadsign(My.Resources.Shizuku_Sign)
        RadioButton2.Checked = True
        element = 1
        Label1.Text = "Selected"
        Button2.Enabled = True
        TextBox1.Text = "トータルチャーム"
        TextBox2.Text = "桜坂しずく"
        des = "トータルチャーム"
        names = "桜坂しずく"
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Dim freewidth As Integer = Me.ClientSize.Width - 30
        Dim freeheight As Integer = Me.ClientSize.Height - 156
        Dim aspect As Double = freewidth / freeheight
        If aspect > 16 / 9 Then 'Too Wide
            freewidth = freeheight * 16 / 9
        Else
            freeheight = freewidth * 9 / 16
        End If
        PictureBox1.Size = New Size(freewidth, freeheight)
        PictureBox2.Size = New Size(freewidth, freeheight)
    End Sub

    Private Sub Button5_Click() Handles Button5.Click
        RadioButton1.Checked = True
        element = 0
        image = New Bitmap(widths, heights)
        signbase = New Bitmap(widths, heights)
        wholebase = New Bitmap(widths, heights)
        modified = New Bitmap(widths, heights)
        completed = New Bitmap(widths, heights)
        smallsign = New Bitmap(widths, heights)
        Label1.Text = "Not Loaded"
        Button2.Enabled = False
        TextBox1.Text = ""
        TextBox2.Text = ""
        des = ""
        names = ""
        PictureBox1.Image = Nothing
    End Sub

    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        widths = TrackBar1.Value * 16
        heights = widths * 9 \ 16
        Label6.Text = "Width: " & widths
        image = New Bitmap(widths, heights)
        signbase = New Bitmap(widths, heights)
        wholebase = New Bitmap(widths, heights)
        modified = New Bitmap(widths, heights)
        completed = New Bitmap(widths, heights)
        smallsign = New Bitmap(widths, heights)
        image = New Bitmap(widths, heights)
        pensize = 6 * widths / 1280 '2~6
        border = 12 * widths / 1280
        PictureBox1.Image = Nothing
        Button5_Click()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click

    End Sub
End Class
