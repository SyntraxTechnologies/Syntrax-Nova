import tkinter as tk
from tkinter import ttk, colorchooser, filedialog
from tkinter import font
import csv

ROWS = 25
COLS = 12


class Spreadsheet:
    def __init__(self, root):
        self.root = root
        self.root.title("Syntrax NovaSheet (Beta 1.01)")

        try:
            root.state("zoomed")
        except:
            w = root.winfo_screenwidth()
            h = root.winfo_screenheight()
            root.geometry(f"{w}x{h}+0+0")

        self.dark_mode = True

        # Defaults
        self.font_size = 12
        self.cell_width = 12
        self.cell_height = 1

        self.selected = None
        self.cells = []

        self.style = ttk.Style()
        self.style.theme_use("clam")

        self.create_toolbar()
        self.create_grid()
        self.apply_theme()

    def create_toolbar(self):
        self.bar = tk.Frame(self.root, padx=5, pady=5)
        self.bar.pack(fill="x")

        tk.Button(self.bar, text="Dark / Light", command=self.toggle_theme).pack(side="left", padx=4)

        tk.Button(self.bar, text="Bold", command=self.make_bold).pack(side="left")
        tk.Button(self.bar, text="Italic", command=self.make_italic).pack(side="left")
        tk.Button(self.bar, text="Underline", command=self.make_underline).pack(side="left")

        tk.Button(self.bar, text="Text Color", command=self.choose_color).pack(side="left", padx=6)

        self.size_box = ttk.Combobox(
            self.bar,
            width=5,
            values=[8, 10, 12, 14, 16, 18, 20, 24, 28, 32]
        )
        self.size_box.set(self.font_size)
        self.size_box.pack(side="left")

        tk.Button(self.bar, text="Set Size", command=self.set_font_size).pack(side="left")

        tk.Button(self.bar, text="Cell +", command=self.increase_cell).pack(side="left", padx=8)
        tk.Button(self.bar, text="Cell -", command=self.decrease_cell).pack(side="left")

        tk.Button(self.bar, text="Save CSV", command=self.save_csv).pack(side="right", padx=4)
        tk.Button(self.bar, text="Load CSV", command=self.load_csv).pack(side="right")

    def create_grid(self):
        self.frame = tk.Frame(self.root)
        self.frame.pack(fill="both", expand=True)

        self.cells = []

        for r in range(ROWS):
            row_cells = []
            for c in range(COLS):

                f = font.Font(size=self.font_size)

                e = tk.Entry(
                    self.frame,
                    width=self.cell_width,
                    font=f
                )

                e.grid(row=r, column=c, ipady=self.cell_height, sticky="nsew")
                e.bind("<FocusIn>", lambda ev, e=e: self.select_cell(e))

                row_cells.append(e)

            self.cells.append(row_cells)

    def select_cell(self, cell):
        self.selected = cell

    def update_font(self, **kwargs):
        if not self.selected:
            return

        current = font.Font(font=self.selected["font"])
        current.configure(**kwargs)
        self.selected.configure(font=current)

    def make_bold(self):
        self.update_font(weight="bold")

    def make_italic(self):
        self.update_font(slant="italic")

    def make_underline(self):
        self.update_font(underline=1)

    def set_font_size(self):
        if not self.selected:
            return

        size = int(self.size_box.get())
        self.update_font(size=size)

    def choose_color(self):
        if not self.selected:
            return

        color = colorchooser.askcolor()[1]
        if color:
            self.selected.configure(fg=color)

    def increase_cell(self):
        self.cell_width += 1
        self.cell_height += 1
        self.rebuild_grid()

    def decrease_cell(self):
        if self.cell_width > 5:
            self.cell_width -= 1
        if self.cell_height > 0:
            self.cell_height -= 1
        self.rebuild_grid()

    def rebuild_grid(self):
        self.frame.destroy()
        self.create_grid()
        self.apply_theme()

    def apply_theme(self):

        if self.dark_mode:
            root_bg = "#1e1e1e"
            toolbar_bg = "#1e1e1e"
            widget_bg = "#2d2d2d"
            cell_bg = "#2b2b2b"
            fg = "white"

            combo_bg = "#2d2d2d"
            combo_fg = "white"

        else:
            root_bg = "white"
            toolbar_bg = "white"
            widget_bg = "#f0f0f0"
            cell_bg = "white"
            fg = "black"

            combo_bg = "white"
            combo_fg = "black"

        self.root.configure(bg=root_bg)
        self.bar.configure(bg=toolbar_bg)
        self.frame.configure(bg=root_bg)

        for widget in self.bar.winfo_children():
            if isinstance(widget, tk.Button):
                widget.configure(bg=widget_bg, fg=fg)

        self.style.configure(
            "TCombobox",
            fieldbackground=combo_bg,
            background=combo_bg,
            foreground=combo_fg
        )

        for row in self.cells:
            for cell in row:
                cell.configure(
                    bg=cell_bg,
                    fg=fg,
                    insertbackground=fg
                )

    def toggle_theme(self):
        self.dark_mode = not self.dark_mode
        self.apply_theme()

    def save_csv(self):
        filename = filedialog.asksaveasfilename(
            defaultextension=".csv",
            filetypes=[("CSV files", "*.csv")]
        )
        if not filename:
            return

        with open(filename, "w", newline="") as f:
            writer = csv.writer(f)
            for row in self.cells:
                writer.writerow([cell.get() for cell in row])

    def load_csv(self):
        filename = filedialog.askopenfilename(
            filetypes=[("CSV files", "*.csv")]
        )
        if not filename:
            return

        with open(filename, "r") as f:
            reader = csv.reader(f)

            for r, row in enumerate(reader):
                if r >= len(self.cells):
                    break
                for c, val in enumerate(row):
                    if c >= len(self.cells[r]):
                        break
                    self.cells[r][c].delete(0, tk.END)
                    self.cells[r][c].insert(0, val)


root = tk.Tk()
Spreadsheet(root)
root.mainloop()