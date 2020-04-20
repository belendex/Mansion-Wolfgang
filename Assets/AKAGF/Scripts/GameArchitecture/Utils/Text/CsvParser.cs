// (c) Francois GUIBERT, Frozax Games
//
// Free to use for personal and commercial uses.
// Tweet @Frozax if you like it.
//
// source: https://github.com/frozax/fgCSVReader

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace AKAGF.GameArchitecture.Utils.Text
{
    public class CsvParser {

        public delegate void ReadLineDelegate(CsvLine line, bool lastLine, params object[] arguments);

        public static void LoadFromFile(string file_name, ReadLineDelegate line_reader, params object[] arguments) {

            try {
                LoadFromString(File.ReadAllText(file_name), line_reader, arguments);
            }
            catch (Exception ex) {
                Debug.LogWarning("File loading error: " + ex.Message + " source:" + ex.Source);
            }
        }

        public static void LoadFromString(string file_contents, ReadLineDelegate line_reader, params object[] arguments)
        {
            int file_length = file_contents.Length;

            // read char by char and when a , or \n, perform appropriate action
            int cur_file_index = 0; // index in the file
            List<string> cur_line = new List<string>(); // current line of data
            int cur_line_number = 0;
            StringBuilder cur_item = new StringBuilder("");
            bool inside_quotes = false; // managing quotes
            while (cur_file_index < file_length)
            {
                char c = file_contents[cur_file_index++];

                switch (c)
                {
                    case '"':
                        if (!inside_quotes)
                        {
                            inside_quotes = true;
                        }
                        else
                        {
                            if (cur_file_index == file_length)
                            {
                                // end of file
                                inside_quotes = false;
                                goto case '\n';
                            }
                            else if (file_contents[cur_file_index] == '"')
                            {
                                // double quote, save one
                                cur_item.Append("\"");
                                cur_file_index++;
                            }
                            else
                            {
                                // leaving quotes section
                                inside_quotes = false;
                            }
                        }
                        break;
                    case '\r':
                        // ignore it completely
                        break;
                    case ',':
                        goto case '\n';
                    case '\n':
                        if (inside_quotes)
                        {
                            // inside quotes, this characters must be included
                            cur_item.Append(c);
                        }
                        else
                        {
                            // end of current item
                            cur_line.Add(cur_item.ToString());
                            cur_item.Length = 0;
                            if (c == '\n' || cur_file_index == file_length)  {
                                bool lastLine = cur_file_index == file_length;
                                // also end of line, call line reader
                                line_reader(new CsvLine(cur_line_number++, cur_line), lastLine, arguments);
                                cur_line.Clear();
                            }
                        }
                        break;
                    default:
                        // other cases, add char
                        cur_item.Append(c);
                        if (cur_file_index == file_length)
                        {
                            goto case '\n';
                        }
                        break;
                }
            }
        }

        //public static List<CsvLine> LoadFromFile(string file_name) {

        //    try {
        //        return LoadFromString(File.ReadAllText(file_name));
        //    }
        //    catch (Exception ex) {
        //        Debug.LogWarning("File loading error: " + ex.Message);
        //        return null;
        //    }

        //}

        //public static List<CsvLine> LoadFromString(string file_contents)
        //{
        //    int file_length = file_contents.Length;

        //    // read char by char and when a , or \n, perform appropriate action
        //    int cur_file_index = 0; // index in the file
        //    List<string> cur_line = new List<string>(); // current line of data
        //    int cur_line_number = 0;
        //    StringBuilder cur_item = new StringBuilder("");
        //    bool inside_quotes = false; // managing quotes
        //    List<CsvLine> linesList = new List<CsvLine>();

        //    while (cur_file_index < file_length)
        //    {
        //        char c = file_contents[cur_file_index++];

        //        switch (c)
        //        {
        //            case '"':
        //                if (!inside_quotes)
        //                {
        //                    inside_quotes = true;
        //                }
        //                else
        //                {
        //                    if (cur_file_index == file_length)
        //                    {
        //                        // end of file
        //                        inside_quotes = false;
        //                        goto case '\n';
        //                    }
        //                    else if (file_contents[cur_file_index] == '"')
        //                    {
        //                        // double quote, save one
        //                        cur_item.Append("\"");
        //                        cur_file_index++;
        //                    }
        //                    else
        //                    {
        //                        // leaving quotes section
        //                        inside_quotes = false;
        //                    }
        //                }
        //                break;
        //            case '\r':
        //                // ignore it completely
        //                break;
        //            case ',':
        //                goto case '\n';
        //            case '\n':
        //                if (inside_quotes)
        //                {
        //                    // inside quotes, this characters must be included
        //                    cur_item.Append(c);
        //                }
        //                else
        //                {
        //                    // end of current item
        //                    cur_line.Add(cur_item.ToString()) ;
        //                    cur_item.Length = 0;
        //                    if (c == '\n' || cur_file_index == file_length)
        //                    {
        //                        // also end of line, call line reader
        //                        linesList.Add(new CsvLine(cur_line_number++, cur_line));
        //                        cur_line.Clear();
        //                    }
        //                }
        //                break;
        //            default:
        //                // other cases, add char
        //                cur_item.Append(c);
        //                if (cur_file_index == file_length)
        //                {
        //                    goto case '\n';
        //                }
        //                break;
        //        }
        //    }

        //    return linesList;
        //}

        //void ReadCSVLine(CsvParser.CsvLine csvLine)
        //{
        //    Debug.Log("\n==> Line " + csvLine.line_index + " " + csvLine.cells.Count + " column(s)");
        //    for (int i = 0; i < csvLine.cells.Count; i++)
        //    {
        //        Debug.Log("Cell " + i + " : " + csvLine.cells[i]);
        //    }
        //}

        public class CsvLine {
            public int line_index;
            public List<string> cells;

            public CsvLine(int line_index, List<string> line_cells) {
                this.line_index = line_index;
                this.cells = new List<string>(line_cells);
            }
        }
    }
}