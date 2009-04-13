<%@ Page Language="vb" %>
<script runat="server">
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        Response.Redirect("~/Home.mvc/Index")
    End Sub
</script>
<!-- Please do not delete this file.  It is used to ensure that ASP.NET MVC is activated by IIS when a user makes a "/" request to the server. -->