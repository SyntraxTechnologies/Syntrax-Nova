import tkinter as tk
from tkinter import colorchooser, simpledialog, ttk, filedialog
import json


class NovaSlide:
    def __init__(self, root):
        self.root = root
        self.root.title("Syntrax NovaSlide (Beta 1.01)")

        try:
            root.state("zoomed")
        except:
            w = root.winfo_screenwidth()
            h = root.winfo_screenheight()
            root.geometry(f"{w}x{h}+0+0")

        # Theme
        self.dark_mode = True

        # Slides
        self.slides = []
        self.current_slide = 0
        self.font_size = 32

        self.create_toolbar()
        self.create_canvas()

        self.add_slide()
        self.apply_theme()

    def create_toolbar(self):
        self.bar = tk.Frame(self.root, padx=5, pady=5)
        self.bar.pack(fill="x")

        tk.Button(self.bar, text="New Slide", command=self.add_slide).pack(side="left")
        tk.Button(self.bar, text="Prev", command=self.prev_slide).pack(side="left")
        tk.Button(self.bar, text="Next", command=self.next_slide).pack(side="left")

        tk.Label(self.bar, text="   ").pack(side="left")

        tk.Button(self.bar, text="Add Text", command=self.add_text).pack(side="left")
        tk.Button(self.bar, text="Text Color", command=self.change_text_color).pack(side="left")

        tk.Label(self.bar, text="Size").pack(side="left")

        self.size_box = ttk.Combobox(
            self.bar,
            width=5,
            values=[16, 20, 24, 28, 32, 40, 48, 56, 64]
        )
        self.size_box.set(self.font_size)
        self.size_box.pack(side="left")

        tk.Button(self.bar, text="Set Size", command=self.set_text_size).pack(side="left")

        tk.Label(self.bar, text="   ").pack(side="left")

        tk.Button(self.bar, text="Slide Background", command=self.change_background).pack(side="left")

        tk.Button(self.bar, text="Save", command=self.save_presentation).pack(side="right", padx=4)
        tk.Button(self.bar, text="Load", command=self.load_presentation).pack(side="right")

        tk.Button(self.bar, text="Dark / Light", command=self.toggle_theme).pack(side="right", padx=4)

        self.slide_label = tk.Label(self.bar, text="Slide 1")
        self.slide_label.pack(side="right", padx=10)

    def create_canvas(self):
        frame = tk.Frame(self.root)
        frame.pack(fill="both", expand=True)

        self.canvas = tk.Canvas(frame, bg="white", highlightthickness=0)
        self.canvas.pack(fill="both", expand=True)

    def add_slide(self):
        default_bg = "#202020" if self.dark_mode else "white"

        slide = {"items": [], "bg": default_bg}
        self.slides.append(slide)
        self.current_slide = len(self.slides) - 1
        self.render_slide()

    def prev_slide(self):
        if self.current_slide > 0:
            self.current_slide -= 1
            self.render_slide()

    def next_slide(self):
        if self.current_slide < len(self.slides) - 1:
            self.current_slide += 1
            self.render_slide()

    def render_slide(self):
        self.canvas.delete("all")

        slide = self.slides[self.current_slide]
        self.canvas.configure(bg=slide["bg"])

        for item in slide["items"]:
            self.canvas.create_text(
                item["x"],
                item["y"],
                text=item["text"],
                fill=item["color"],
                font=("Arial", item["size"])
            )

        self.slide_label.config(
            text=f"Slide {self.current_slide + 1}/{len(self.slides)}"
        )

    # -----------------------------
    # TEXT
    # -----------------------------
    def add_text(self):
        text = simpledialog.askstring("Text", "Enter text:")
        if not text:
            return

        default_color = "white" if self.dark_mode else "black"

        item = {
            "text": text,
            "x": 400,
            "y": 300,
            "color": default_color,
            "size": self.font_size
        }

        self.slides[self.current_slide]["items"].append(item)
        self.render_slide()

    def set_text_size(self):
        self.font_size = int(self.size_box.get())

    def change_text_color(self):
        color = colorchooser.askcolor()[1]
        if not color:
            return

        slide = self.slides[self.current_slide]
        if slide["items"]:
            slide["items"][-1]["color"] = color

        self.render_slide()

    def change_background(self):
        color = colorchooser.askcolor()[1]
        if not color:
            return

        self.slides[self.current_slide]["bg"] = color
        self.render_slide()

    def save_presentation(self):
        file = filedialog.asksaveasfilename(
            defaultextension=".json",
            filetypes=[("JSON files", "*.json")]
        )
        if not file:
            return

        with open(file, "w") as f:
            json.dump(self.slides, f)

    def load_presentation(self):
        file = filedialog.askopenfilename(
            filetypes=[("JSON files", "*.json")]
        )
        if not file:
            return

        with open(file, "r") as f:
            self.slides = json.load(f)

        self.current_slide = 0
        self.render_slide()

    def apply_theme(self):
        if self.dark_mode:
            root_bg = "#1e1e1e"
            toolbar_bg = "#1e1e1e"
            widget_bg = "#2d2d2d"
            fg = "white"
        else:
            root_bg = "white"
            toolbar_bg = "white"
            widget_bg = "#f0f0f0"
            fg = "black"

        self.root.configure(bg=root_bg)
        self.bar.configure(bg=toolbar_bg)

        for widget in self.bar.winfo_children():
            if isinstance(widget, (tk.Button, tk.Label)):
                widget.configure(bg=widget_bg, fg=fg)

        slide = self.slides[self.current_slide]
        if slide["bg"] in ["white", "#202020"]:
            slide["bg"] = "#202020" if self.dark_mode else "white"

        self.render_slide()

    def toggle_theme(self):
        self.dark_mode = not self.dark_mode
        self.apply_theme()

root = tk.Tk()
NovaSlide(root)
root.mainloop()