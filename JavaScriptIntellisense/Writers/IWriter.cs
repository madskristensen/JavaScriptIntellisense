using System;
namespace JqueryIntellisense
{
    interface IWriter
    {
        int Count { get; set; }
        void StartWriting(System.Collections.Generic.List<Entry> list, System.Text.StringBuilder sb);
        string Name { get; }
    }
}
