<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NHaml.Samples.MonoRail._Default" %>

<script runat="server">
  protected override void OnLoad(EventArgs e)
  {
    Response.Redirect("~/Home/index.rails");
    base.OnLoad(e);
  }
</script>

