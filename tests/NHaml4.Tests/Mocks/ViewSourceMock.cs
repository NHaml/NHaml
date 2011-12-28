using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.TemplateResolution;

namespace NHaml4.Tests.Mocks
{
    public class ViewSourceMock : IViewSource
    {
        private System.IO.StreamReader _streamReader;
        private string _path;

        public ViewSourceMock(System.IO.StreamReader streamReader, string path)
        {
            this._streamReader = streamReader;
            this._path = path;
        }

        public System.IO.StreamReader GetStreamReader()
        {
            return _streamReader;
        }

        public string Path
        {
            get { return _path; }
        }

        public bool IsModified
        {
            get { return true; }
        }

        public string GetClassName()
        {
            return "mockClassName";
        }
    }
}
