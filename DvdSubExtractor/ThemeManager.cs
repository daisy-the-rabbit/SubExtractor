namespace DvdSubExtractor;

internal static class ThemeManager
{
    const int WM_THEMECHANGED = 0x031A;
    const int WM_SYSCOLORCHANGE = 0x0015;

    public const string SystemTheme = "System";
    public const string LightTheme = "Light";
    public const string DarkTheme = "Dark";

    public static string NormalizeTheme(string theme)
    {
        if (string.Equals(theme, DarkTheme, StringComparison.OrdinalIgnoreCase))
        {
            return DarkTheme;
        }
        if (string.Equals(theme, LightTheme, StringComparison.OrdinalIgnoreCase))
        {
            return LightTheme;
        }
        return SystemTheme;
    }

    public static void ApplyThemeToOpenForms(string theme)
    {
        ApplyNativeTheme(NormalizeTheme(theme));
        foreach (Form form in Application.OpenForms)
        {
            ApplyThemeToControlTree(form);
            form.Invalidate(true);
            form.Update();
        }
    }

    public static void ApplyTheme(Control root, string theme)
    {
        ApplyNativeTheme(NormalizeTheme(theme));
        if (root != null)
        {
            ApplyThemeToControlTree(root);
        }
    }

    static void ApplyNativeTheme(string theme)
    {
        SystemColorMode mode = SystemColorMode.System;
        if (theme == DarkTheme)
        {
            mode = SystemColorMode.Dark;
        }
        else if (theme == LightTheme)
        {
            mode = SystemColorMode.Classic;
        }
        Application.SetColorMode(mode);
    }

    static void ApplyThemeToControlTree(Control control)
    {
        if (control == null)
        {
            return;
        }

        if (control.BackColor.IsSystemColor || control.BackColor == Control.DefaultBackColor)
        {
            control.ResetBackColor();
        }

        if (control.ForeColor.IsSystemColor || control.ForeColor == Control.DefaultForeColor)
        {
            control.ResetForeColor();
        }

        if (control is ToolStrip toolStrip)
        {
            foreach (ToolStripItem item in toolStrip.Items)
            {
                if (item.BackColor.IsEmpty || item.BackColor.IsSystemColor)
                {
                    item.ResetBackColor();
                }

                if (item.ForeColor.IsEmpty || item.ForeColor.IsSystemColor)
                {
                    item.ResetForeColor();
                }
            }
        }

        foreach (Control child in control.Controls)
        {
            ApplyThemeToControlTree(child);
        }

        if (control.IsHandleCreated)
        {
            SendMessage(control.Handle, WM_THEMECHANGED, IntPtr.Zero, IntPtr.Zero);
            SendMessage(control.Handle, WM_SYSCOLORCHANGE, IntPtr.Zero, IntPtr.Zero);
            control.Invalidate(true);
        }
    }

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
}

