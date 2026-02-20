import tkinter as tk
from tkinter import filedialog, colorchooser, ttk, simpledialog
from tkinter import font


class NovaWrite:
    def __init__(self, root):
        self.root = root
        self.root.title("Syntrax NovaWrite (Beta 1.0)")

        try:
            root.state("zoomed")
        except:
            w = root.winfo_screenwidth()
            h = root.winfo_screenheight()
            root.geometry(f"{w}x{h}+0+0")

        self.dark_mode = True
        self.current_font_size = 12
        self.current_font_family = "Arial"

        self.create_toolbar()
        self.create_text_area()
        self.create_status_bar()
        self.apply_theme()
        self.bind_shortcuts()

    def create_toolbar(self):
        bar = tk.Frame(self.root, padx=5, pady=5)
        bar.pack(fill="x")

        tk.Button(bar, text="Open", command=self.open_file).pack(side="left")
        tk.Button(bar, text="Save", command=self.save_file).pack(side="left", padx=4)

        tk.Label(bar, text="   ").pack(side="left")

        fonts = list(font.families())
        self.font_box = ttk.Combobox(bar, values=fonts, width=20)
        self.font_box.set(self.current_font_family)
        self.font_box.pack(side="left")
        tk.Button(bar, text="Set Font", command=self.set_font_family).pack(side="left")

        self.size_box = ttk.Combobox(
            bar,
            width=5,
            values=[8,10,12,14,16,18,20,24,28,32]
        )
        self.size_box.set(self.current_font_size)
        self.size_box.pack(side="left")

        tk.Button(bar, text="Size", command=self.set_font_size).pack(side="left")

        tk.Button(bar, text="Bold", command=lambda: self.toggle_tag("bold")).pack(side="left")
        tk.Button(bar, text="Italic", command=lambda: self.toggle_tag("italic")).pack(side="left")
        tk.Button(bar, text="Underline", command=lambda: self.toggle_tag("underline")).pack(side="left")

        tk.Button(bar, text="Color", command=self.choose_color).pack(side="left", padx=6)

        tk.Button(bar, text="Left", command=lambda: self.align("left")).pack(side="left")
        tk.Button(bar, text="Center", command=lambda: self.align("center")).pack(side="left")
        tk.Button(bar, text="Right", command=lambda: self.align("right")).pack(side="left")

        tk.Label(bar, text="   ").pack(side="left")

        tk.Button(bar, text="• List", command=self.bullet_list).pack(side="left")

        tk.Button(bar, text="Dark / Light", command=self.toggle_theme).pack(side="right")

    def create_text_area(self):
        frame = tk.Frame(self.root)
        frame.pack(fill="both", expand=True)

        scroll = tk.Scrollbar(frame)
        scroll.pack(side="right", fill="y")

        self.text = tk.Text(
            frame,
            wrap="word",
            undo=True,
            font=(self.current_font_family, self.current_font_size)
        )

        self.text.pack(fill="both", expand=True)
        scroll.config(command=self.text.yview)
        self.text.config(yscrollcommand=scroll.set)

        self.text.bind("<KeyRelease>", self.update_word_count)

        self.configure_tags()

    def create_status_bar(self):
        self.status = tk.Label(self.root, text="Words: 0", anchor="e")
        self.status.pack(fill="x")

    def update_word_count(self, event=None):
        text = self.text.get("1.0", tk.END)
        words = len(text.split())
        self.status.config(text=f"Words: {words}")

    def configure_tags(self):
        size = self.current_font_size
        family = self.current_font_family

        self.text.tag_configure("bold", font=(family, size, "bold"))
        self.text.tag_configure("italic", font=(family, size, "italic"))
        self.text.tag_configure("underline", font=(family, size, "underline"))

        self.text.tag_configure("left", justify="left")
        self.text.tag_configure("center", justify="center")
        self.text.tag_configure("right", justify="right")

    def toggle_tag(self, tag):
        try:
            start = self.text.index("sel.first")
            end = self.text.index("sel.last")

            if tag in self.text.tag_names("sel.first"):
                self.text.tag_remove(tag, start, end)
            else:
                self.text.tag_add(tag, start, end)
        except:
            pass

    def set_font_size(self):
        self.current_font_size = int(self.size_box.get())
        self.text.configure(font=(self.current_font_family, self.current_font_size))
        self.configure_tags()

    def set_font_family(self):
        self.current_font_family = self.font_box.get()
        self.text.configure(font=(self.current_font_family, self.current_font_size))
        self.configure_tags()

    def align(self, mode):
        try:
            start = self.text.index("sel.first")
            end = self.text.index("sel.last")
            self.text.tag_add(mode, start, end)
        except:
            pass

    def bullet_list(self):
        try:
            start = self.text.index("sel.first linestart")
            end = self.text.index("sel.last lineend")

            lines = self.text.get(start, end).split("\n")

            new = "\n".join("• " + l for l in lines)

            self.text.delete(start, end)
            self.text.insert(start, new)
        except:
            pass

    def choose_color(self):
        try:
            color = colorchooser.askcolor()[1]
            if not color:
                return

            tag = f"color_{color}"
            self.text.tag_configure(tag, foreground=color)

            start = self.text.index("sel.first")
            end = self.text.index("sel.last")

            self.text.tag_add(tag, start, end)
        except:
            pass

    def find_text(self):
        word = simpledialog.askstring("Find", "Enter text:")
        if not word:
            return

        start = "1.0"
        while True:
            pos = self.text.search(word, start, stopindex=tk.END)
            if not pos:
                break
            end = f"{pos}+{len(word)}c"
            self.text.tag_add("find", pos, end)
            self.text.tag_config("find", background="yellow")
            start = end

    def open_file(self):
        file = filedialog.askopenfilename(
            filetypes=[("Text files", "*.txt"), ("All files", "*.*")]
        )
        if not file:
            return

        with open(file, "r", encoding="utf-8") as f:
            content = f.read()

        self.text.delete("1.0", tk.END)
        self.text.insert(tk.END, content)

    def save_file(self):
        file = filedialog.asksaveasfilename(
            defaultextension=".txt",
            filetypes=[("Text files", "*.txt")]
        )
        if not file:
            return

        with open(file, "w", encoding="utf-8") as f:
            f.write(self.text.get("1.0", tk.END))

    def bind_shortcuts(self):
        self.root.bind("<Control-f>", lambda e: self.find_text())

    def apply_theme(self):
        if self.dark_mode:
            bg = "#1e1e1e"
            fg = "white"
        else:
            bg = "white"
            fg = "black"

        self.root.configure(bg=bg)
        self.text.configure(bg=bg, fg=fg, insertbackground=fg)

    def toggle_theme(self):
        self.dark_mode = not self.dark_mode
        self.apply_theme()

root = tk.Tk()
NovaWrite(root)
root.mainloop()