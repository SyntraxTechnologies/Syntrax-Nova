import tkinter as tk
from tkinter import ttk, filedialog, colorchooser, simpledialog
import json
import xml.etree.ElementTree as ET

APP_NAME = "Syntrax NovaBook (Beta 1.01)"
AUTOSAVE_FILE = "autosave.xml"
AUTOSAVE_INTERVAL = 60000   # ms

class NovaBook:
    def __init__(self, root):
        self.root = root
        self.root.title(APP_NAME)
        self.root.geometry("1200x800")

        self.dark_mode = True
        self.notebook = {"pages": []}

        self.create_toolbar()
        self.create_tabs()
        self.create_statusbar()
        self.bind_shortcuts()
        self.apply_theme()

        self.root.after(AUTOSAVE_INTERVAL, self.autosave)
        self.add_page()

    def create_toolbar(self):
        self.toolbar = tk.Frame(self.root)
        self.toolbar.pack(fill="x")

        tk.Button(self.toolbar, text="New Page", command=self.add_page).pack(side="left")
        tk.Button(self.toolbar, text="Save JSON", command=self.save_json).pack(side="left")
        tk.Button(self.toolbar, text="Load JSON", command=self.load_json).pack(side="left")
        tk.Button(self.toolbar, text="Save XML", command=self.save_xml).pack(side="left")
        tk.Button(self.toolbar, text="Load XML", command=self.load_xml).pack(side="left")
        tk.Label(self.toolbar, text="   ").pack(side="left")

        tk.Button(self.toolbar, text="Bold", command=lambda: self.toggle_tag("bold")).pack(side="left")
        tk.Button(self.toolbar, text="Italic", command=lambda: self.toggle_tag("italic")).pack(side="left")
        tk.Button(self.toolbar, text="Underline", command=lambda: self.toggle_tag("underline")).pack(side="left")
        tk.Button(self.toolbar, text="Color", command=self.choose_color).pack(side="left")
        tk.Label(self.toolbar, text="Size").pack(side="left")

        self.size_box = ttk.Combobox(
            self.toolbar,
            width=5,
            values=[10,12,14,16,18,20,24,28,32]
        )
        self.size_box.set(12)
        self.size_box.pack(side="left")
        tk.Button(self.toolbar, text="Set Size", command=self.set_font_size).pack(side="left")

        tk.Button(self.toolbar, text="Search", command=self.search_text).pack(side="right")
        tk.Button(self.toolbar, text="Dark/Light", command=self.toggle_theme).pack(side="right")

    def create_tabs(self):
        self.tabs = ttk.Notebook(self.root)
        self.tabs.pack(fill="both", expand=True)
        self.tabs.bind("<<NotebookTabChanged>>", self.update_status)

    def create_statusbar(self):
        self.status = tk.Label(self.root, text="Ready", anchor="w")
        self.status.pack(fill="x")

    def add_page(self):
        title = simpledialog.askstring("Page name", "Enter page title:")
        if not title:
            title = f"Page {len(self.notebook['pages'])+1}"

        frame = tk.Frame(self.tabs)
        text = tk.Text(frame, wrap="word", undo=True, font=("Arial", 12))
        text.pack(fill="both", expand=True)

        self.configure_tags(text)
        self.tabs.add(frame, text=title)

        self.notebook["pages"].append({"title": title, "widget": text})
        self.tabs.select(frame)

        text.bind("<KeyRelease>", self.update_status)
        text.bind("<ButtonRelease>", self.update_status)

        self.apply_theme_to_text(text)

    def get_current_text_widget(self):
        if not self.notebook["pages"]:
            return None
        index = self.tabs.index("current")
        return self.notebook["pages"][index]["widget"]

    def configure_tags(self, text):
        text.tag_configure("bold", font=("Arial", 12, "bold"))
        text.tag_configure("italic", font=("Arial", 12, "italic"))
        text.tag_configure("underline", font=("Arial", 12, "underline"))
        text.tag_configure("search", background="yellow")

    def toggle_tag(self, tag):
        text = self.get_current_text_widget()
        if not text:
            return
        try:
            start = text.index("sel.first")
            end = text.index("sel.last")
            if tag in text.tag_names("sel.first"):
                text.tag_remove(tag, start, end)
            else:
                text.tag_add(tag, start, end)
        except:
            pass

    def choose_color(self):
        text = self.get_current_text_widget()
        if not text:
            return
        try:
            color = colorchooser.askcolor()[1]
            if not color:
                return
            tag = f"color_{color}"
            text.tag_configure(tag, foreground=color)
            start = text.index("sel.first")
            end = text.index("sel.last")
            text.tag_add(tag, start, end)
        except:
            pass

    def set_font_size(self):
        text = self.get_current_text_widget()
        if not text:
            return
        size = int(self.size_box.get())
        text.configure(font=("Arial", size))

    def search_text(self):
        text = self.get_current_text_widget()
        if not text:
            return
        term = simpledialog.askstring("Search", "Enter search text:")
        if not term:
            return
        start = "1.0"
        text.tag_remove("search", "1.0", tk.END)
        while True:
            pos = text.search(term, start, stopindex=tk.END)
            if not pos:
                break
            end = f"{pos}+{len(term)}c"
            text.tag_add("search", pos, end)
            start = end

    def collect_data(self):
        return {"pages": [{"title": p["title"], "content": p["widget"].get("1.0", tk.END)} for p in self.notebook["pages"]]}

    def save_json(self):
        file = filedialog.asksaveasfilename(defaultextension=".json", filetypes=[("JSON", "*.json")])
        if not file: return
        with open(file, "w") as f:
            json.dump(self.collect_data(), f, indent=2)

    def load_json(self):
        file = filedialog.askopenfilename(filetypes=[("JSON", "*.json")])
        if not file: return
        with open(file, "r") as f:
            data = json.load(f)
        self.load_data_into_ui(data)

    def save_xml(self):
        file = filedialog.asksaveasfilename(defaultextension=".xml", filetypes=[("XML", "*.xml")])
        if not file: return
        data = self.collect_data()
        root = ET.Element("notebook")
        for page in data["pages"]:
            p = ET.SubElement(root, "page")
            ET.SubElement(p, "title").text = page["title"]
            ET.SubElement(p, "content").text = page["content"]
        tree = ET.ElementTree(root)
        tree.write(file, encoding="utf-8", xml_declaration=True)

    def load_xml(self):
        file = filedialog.askopenfilename(filetypes=[("XML", "*.xml")])
        if not file: return
        tree = ET.parse(file)
        root = tree.getroot()
        data = {"pages": [{"title": p.find("title").text, "content": p.find("content").text} for p in root.findall("page")]}
        self.load_data_into_ui(data)

    def load_data_into_ui(self, data):
        for tab in self.tabs.tabs():
            self.tabs.forget(tab)
        self.notebook["pages"].clear()
        for page in data["pages"]:
            self.add_page_manual(page["title"], page["content"])

    def add_page_manual(self, title, content):
        frame = tk.Frame(self.tabs)
        text = tk.Text(frame, wrap="word", undo=True, font=("Arial", 12))
        text.pack(fill="both", expand=True)
        self.configure_tags(text)
        text.insert("1.0", content)
        self.tabs.add(frame, text=title)
        self.notebook["pages"].append({"title": title, "widget": text})
        self.apply_theme_to_text(text)

    def autosave(self):
        try:
            data = self.collect_data()
            root = ET.Element("notebook")
            for page in data["pages"]:
                p = ET.SubElement(root, "page")
                ET.SubElement(p, "title").text = page["title"]
                ET.SubElement(p, "content").text = page["content"]
            tree = ET.ElementTree(root)
            tree.write(AUTOSAVE_FILE, encoding="utf-8", xml_declaration=True)
        except:
            pass
        self.root.after(AUTOSAVE_INTERVAL, self.autosave)

    def apply_theme(self):
        if self.dark_mode:
            self.bg = "#1e1e1e"
            self.fg = "white"
            self.btn_bg = "#2e2e2e"
            self.btn_fg = "white"
            self.entry_bg = "#3e3e3e"
            self.entry_fg = "white"
        else:
            self.bg = "white"
            self.fg = "black"
            self.btn_bg = "lightgray"
            self.btn_fg = "black"
            self.entry_bg = "white"
            self.entry_fg = "black"

        # Root window
        self.root.configure(bg=self.bg)
        # Toolbar widgets
        for child in self.toolbar.winfo_children():
            if isinstance(child, tk.Button):
                child.configure(bg=self.btn_bg, fg=self.btn_fg, activebackground=self.btn_bg, activeforeground=self.btn_fg)
            elif isinstance(child, tk.Label):
                child.configure(bg=self.bg, fg=self.fg)
            elif isinstance(child, ttk.Combobox):
                style = ttk.Style()
                style.theme_use('default')
                style.configure("TCombobox", fieldbackground=self.entry_bg, background=self.entry_bg, foreground=self.entry_fg)

        for page in self.notebook["pages"]:
            self.apply_theme_to_text(page["widget"])

        self.status.configure(bg=self.bg, fg=self.fg)

    def apply_theme_to_text(self, text):
        text.configure(bg=self.bg, fg=self.fg, insertbackground=self.fg, selectbackground="#555555" if self.dark_mode else "#c0c0ff")

    def toggle_theme(self):
        self.dark_mode = not self.dark_mode
        self.apply_theme()

    def update_status(self, event=None):
        text = self.get_current_text_widget()
        if not text: return
        pos = text.index(tk.INSERT)
        line, col = pos.split(".")
        self.status.config(text=f"Line {line} | Column {col}")

    def bind_shortcuts(self):
        self.root.bind("<Control-b>", lambda e: self.toggle_tag("bold"))
        self.root.bind("<Control-i>", lambda e: self.toggle_tag("italic"))
        self.root.bind("<Control-u>", lambda e: self.toggle_tag("underline"))
        self.root.bind("<Control-f>", lambda e: self.search_text())

root = tk.Tk()
app = NovaBook(root)
root.mainloop()