//------------------------------------------------------------------------------
// <copyright file="TextModifierWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace TextConvert
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for TextModifierWindowControl.
    /// </summary>
    public partial class TextModifierWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextModifierWindowControl"/> class.
        /// </summary>
        public TextModifierWindowControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "TextModifierWindow");
        }


        private void toStringBuilderCommand_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder output = new StringBuilder();


            for (int row = 0; row < sourceText.LineCount; row++)
            {
                string line = sourceText.GetLineText(row).Replace("\n", string.Empty).Replace("\r", string.Empty);

                string escaped = line.Replace("\"", "\\\"");
                output.AppendLine(String.Format("x.AppendLine(\"{0}\");", escaped));
            }

            this.destText.Text = output.ToString();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder x = new StringBuilder();

            x.Append("var x = new string[] {");

            for (int row = 0; row < sourceText.LineCount; row++)
            {
                string line = sourceText.GetLineText(row).Replace("\n", string.Empty).Replace("\r", string.Empty);
                x.AppendFormat("\"{0}\",", line);
            }

            x.Remove(x.Length - 1, 1);

            x.Append("};");

            destText.Text = x.ToString();

        }

        private void toDynamicSqlCommand_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder output = new StringBuilder();
            bool statementBefore = false;
            bool insideBlock = false;


            for (int row = 0; row < sourceText.LineCount; row++)
            {
                string line = sourceText.GetLineText(row).Replace("\n", string.Empty).Replace("\r", string.Empty);
                if (line.StartsWith("ALTER") || line.StartsWith("CREATE") || line.StartsWith("DROP"))
                {
                    if (statementBefore)
                    {
                        output.Append("'");

                        output.AppendLine("");

                        output.AppendLine("SET @currentExecutingSQL = @SQL");

                        output.AppendLine("exec (@sql)");
                    }

                    statementBefore = true;


                    output.AppendLine("set @sql = '");
                }
                if (line.StartsWith("GO"))
                {
                    // skip
                }
                else if (line.TrimStart().StartsWith("PRINT"))
                {
                    // skip
                }
                else
                {
                    output.AppendLine(line.Replace("'", "''"));
                }
            }

            output.Append("'");

            output.AppendLine("");

            output.AppendLine("SET @currentExecutingSQL = @SQL");

            output.AppendLine("exec (@sql)");


            destText.Text = output.ToString();
        }

        private void toinCommand_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder x = new StringBuilder();

            x.Append("IN (");

            for (int row = 0; row < sourceText.LineCount; row++)
            {
                string line = sourceText.GetLineText(row).Replace("\n", string.Empty).Replace("\r", string.Empty);

                if (this.quoted.IsChecked == true)
                {
                    x.AppendFormat("'{0}',", line.Trim());
                }
                else
                {
                    x.AppendFormat("{0},", line.Trim());
                }

            }

            x.Remove(x.Length - 1, 1);
            x.Append(")");

            destText.Text = x.ToString();

        }

        private void removeCRLFCommand_Click(object sender, RoutedEventArgs e)
        {
            destText.Text = sourceText.Text.TrimEnd().Replace("\n", string.Empty).Replace("\r", string.Empty);
        }

        private void equalCommand_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder output = new StringBuilder();

            for (int row = 0; row < sourceText.LineCount; row++)
            {
                string line = sourceText.GetLineText(row).Replace("\n", string.Empty).Replace("\r", string.Empty);

                int firstBracket = line.IndexOf('[');

                if (firstBracket >= 0)
                {
                    int endBracket = line.IndexOf(']');

                    int length = (endBracket - firstBracket) + 1;

                    output.AppendLine(String.Format("{0} = x.{0},", line.Substring(firstBracket, length)));
                }
            }

            destText.Text = output.ToString();
        }

        private void columnNameCommand_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder output = new StringBuilder();

            for (int row = 0; row < sourceText.LineCount; row++)
            {
                string line = sourceText.GetLineText(row).Replace("\n", string.Empty).Replace("\r", string.Empty);

                int firstBracket = line.IndexOf('[');

                if (firstBracket >= 0)
                {
                    int endBracket = line.IndexOf(']');

                    int length = (endBracket - firstBracket) + 1;

                    output.AppendLine(String.Format("{0},", line.Substring(firstBracket, length)));
                }

                destText.Text = output.ToString();
            }
        }
    }
}