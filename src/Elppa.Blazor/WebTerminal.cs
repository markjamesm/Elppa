using System.Text;
using Elppa.Core;

namespace Elppa.Blazor;

public sealed class WebTerminal : ITerminal
{
    private const int Columns = 40, Rows = 24;

    private readonly char[][] _rows =
        [.. Enumerable.Range(0, Rows).Select(_ => new string(' ', Columns).ToCharArray())];

    private int _row, _col;

    public bool IsDirty { get; private set; } = true;

    public void WriteCharacter(byte asciiCode)
    {
        switch (asciiCode)
        {
            case 0x0D: NewLine(); break;
            case 0x5F: // Rubout
                if (_col > 0) _rows[_row][--_col] = ' ';
                break;
            case >= 0x20 and < 0x7F:
                _rows[_row][_col] = (char)asciiCode;
                if (++_col == Columns) NewLine();
                break;
            default: return; // Ignore control codes.
        }

        IsDirty = true;
    }

    private void NewLine()
    {
        _col = 0;

        if (++_row < Rows)
        {
            return;
        }

        _row = Rows - 1;
        var top = _rows[0];
        Array.Copy(_rows, 1, _rows, 0, Rows - 1); // Scroll up.
        Array.Fill(top, ' ');
        _rows[Rows - 1] = top;
    }

    public string Render()
    {
        IsDirty = false;
        var sb = new StringBuilder();

        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Columns; c++)
            {
                sb.Append(r == _row && c == _col ? '@' : _rows[r][c]); // The Apple I cursor.   
            }

            if (r < Rows - 1)
            {
                sb.Append('\n');
            }
        }

        return sb.ToString();
    }
}