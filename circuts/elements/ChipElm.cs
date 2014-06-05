using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {
	
	public abstract class ChipElm : CircuitElm {
		public int csize, cspc, cspc2;
		public int bits;
		public int FLAG_SMALL = 1;
		public int FLAG_FLIP_X = 1024;
		public int FLAG_FLIP_Y = 2048;

		public ChipElm(int xx, int yy,CirSim s) : base(xx, yy,s) {
			if (needsBits()) {
				bits = (this is DecadeElm) ? 10 : 4;
			}
			noDiagonal = true;
			setupPins();
			setSize(sim.smallGridCheckItem ? 1 : 2);
		}

		public virtual bool needsBits() {
			return false;
		}

		public void setSize(int s) {
			csize = s;
			cspc = 8 * s;
			cspc2 = cspc * 2;
			flags &= ~FLAG_SMALL;
			flags |= (s == 1) ? FLAG_SMALL : 0;
		}

		public virtual void setupPins(){ }

		/*void draw(Graphics g) {
			drawChip(g);
		}

		void drawChip(Graphics g) {
			int i;
			Font f = new Font("SansSerif", 0, 10 * csize);
			g.setFont(f);
			FontMetrics fm = g.getFontMetrics();
			for (i = 0; i != getPostCount(); i++) {
				Pin p = pins[i];
				setVoltageColor(g, volts[i]);
				Point a = p.post;
				Point b = p.stub;
				drawThickLine(g, a, b);
				p.curcount = updateDotCount(p.current, p.curcount);
				drawDots(g, b, a, p.curcount);
				if (p.bubble) {
					g.setColor(sim.printableCheckItem.getState() ? Color.white
							: Color.black);
					drawThickCircle(g, p.bubbleX, p.bubbleY, 1);
					g.setColor(lightGrayColor);
					drawThickCircle(g, p.bubbleX, p.bubbleY, 3);
				}
				g.setColor(whiteColor);
				int sw = fm.stringWidth(p.text);
				g.drawString(p.text, p.textloc.x - sw / 2,
						p.textloc.y + fm.getAscent() / 2);
				if (p.lineOver) {
					int ya = p.textloc.y - fm.getAscent() / 2;
					g.drawLine(p.textloc.x - sw / 2, ya, p.textloc.x + sw / 2, ya);
				}
			}
			g.setColor(needsHighlight() ? selectColor : lightGrayColor);
			drawThickPolygon(g, rectPointsX, rectPointsY, 4);
			if (clockPointsX != null) {
				g.drawPolyline(clockPointsX, clockPointsY, 3);
			}
			for (i = 0; i != getPostCount(); i++) {
				drawPost(g, pins[i].post.x, pins[i].post.y, nodes[i]);
			}
		}*/

		public int[] rectPointsX, rectPointsY;
		public int[] clockPointsX, clockPointsY;
		public Pin[] pins;
		public int sizeX, sizeY;
		public bool lastClock;

		public override void setPoints() {
			if (x2 - x > sizeX * cspc2 && this == sim.dragElm) {
				setSize(2);
			}
			int x0 = x + cspc2;
			int y0 = y;
			int xr = x0 - cspc;
			int yr = y0 - cspc;
			int xs = sizeX * cspc2;
			int ys = sizeY * cspc2;
			rectPointsX = new int[] { xr, xr + xs, xr + xs, xr };
			rectPointsY = new int[] { yr, yr, yr + ys, yr + ys };
			//setBbox(xr, yr, rectPointsX[2], rectPointsY[2]);
			int i;
			for (i = 0; i != getPostCount(); i++) {
				Pin p = pins[i];
				switch (p.side) {
				case 0:
					p.setPoint(x0, y0, 1, 0, 0, -1, 0, 0);
					break;
				case 1:
					p.setPoint(x0, y0, 1, 0, 0, 1, 0, ys - cspc2);
					break;
				case 2:
					p.setPoint(x0, y0, 0, 1, -1, 0, 0, 0);
					break;
				case 3:
					p.setPoint(x0, y0, 0, 1, 1, 0, xs - cspc2, 0);
					break;
				}
			}
		}

		public override Point getPost(int n) {
			return pins[n].post;
		}

		public override void setVoltageSource(int j, int vs) {
			int i;
			for (i = 0; i != getPostCount(); i++) {
				Pin p = pins[i];
				if (p.output && j-- == 0) {
					p.voltSource = vs;
					return;
				}
			}
			//System.out.println("setVoltageSource failed for " + this);
		}

		public override void stamp() {
			int i;
			for (i = 0; i != getPostCount(); i++) {
				Pin p = pins[i];
				if (p.output) {
					sim.stampVoltageSource(0, nodes[i], p.voltSource);
				}
			}
		}

		public virtual void execute(){ }

		public override void doStep() {
			int i;
			for (i = 0; i != getPostCount(); i++) {
				Pin p = pins[i];
				if (!p.output) {
					p.value = volts[i] > 2.5;
				}
			}
			execute();
			for (i = 0; i != getPostCount(); i++) {
				Pin p = pins[i];
				if (p.output) {
					sim.updateVoltageSource(0, nodes[i], p.voltSource, p.value ? 5 : 0);
				}
			}
		}

		public override void reset() {
			int i;
			for (i = 0; i != getPostCount(); i++) {
				pins[i].value = false;
				pins[i].curcount = 0;
				volts[i] = 0;
			}
			lastClock = false;
		}

		public override void getInfo(String[] arr) {
			arr[0] = getChipName();
			int i, a = 1;
			for (i = 0; i != getPostCount(); i++) {
				Pin p = pins[i];
				if (arr[a] != null) {
					arr[a] += "; ";
				} else {
					arr[a] = "";
				}
				String t = p.text;
				if (p.lineOver) {
					t += '\'';
				}
				if (p.clock) {
					t = "Clk";
				}
				arr[a] += t + " = " + getVoltageText(volts[i]);
				if (i % 2 == 1) {
					a++;
				}
			}
		}

		public override void setCurrent(int x, double c) {
			int i;
			for (i = 0; i != getPostCount(); i++) {
				if (pins[i].output && pins[i].voltSource == x) {
					pins[i].current = c;
				}
			}
		}

		public virtual String getChipName() {
			return "chip";
		}

		public override bool getConnection(int n1, int n2) {
			return false;
		}

		public override bool hasGroundConnection(int n1) {
			return pins[n1].output;
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				EditInfo ei = new EditInfo("", 0, -1, -1);
				ei.checkbox = new Checkbox("Flip X", (flags & FLAG_FLIP_X) != 0);
				return ei;
			}
			if (n == 1) {
				EditInfo ei = new EditInfo("", 0, -1, -1);
				ei.checkbox = new Checkbox("Flip Y", (flags & FLAG_FLIP_Y) != 0);
				return ei;
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			if (n == 0) {
				if (ei.checkbox.getState()) {
					flags |= FLAG_FLIP_X;
				} else {
					flags &= ~FLAG_FLIP_X;
				}
				setPoints();
			}
			if (n == 1) {
				if (ei.checkbox.getState()) {
					flags |= FLAG_FLIP_Y;
				} else {
					flags &= ~FLAG_FLIP_Y;
				}
				setPoints();
			}
		}*/

		public int SIDE_N = 0;
		public int SIDE_S = 1;
		public int SIDE_W = 2;
		public int SIDE_E = 3;

		public class Pin {

			public Pin(int p, int s, String t,ChipElm element) {
				pos = p;
				side = s;
				text = t;
				elm = element;
			}
			
			public ChipElm elm;
			public Point post, stub;
			public Point textloc;
			public int pos, side, voltSource, bubbleX, bubbleY;
			public String text;
			public bool lineOver, bubble, clock, output, value, state;
			public double curcount, current;
			
			public void setPoint(int px, int py, int dx, int dy, int dax, int day, int sx, int sy) {
				if ((elm.flags & elm.FLAG_FLIP_X) != 0) {
					dx = -dx;
					dax = -dax;
					px += elm.cspc2 * (elm.sizeX - 1);
					sx = -sx;
				}
				if ((elm.flags & elm.FLAG_FLIP_Y) != 0) {
					dy = -dy;
					day = -day;
					py += elm.cspc2 * (elm.sizeY - 1);
					sy = -sy;
				}
				int xa = px + elm.cspc2 * dx * pos + sx;
				int ya = py + elm.cspc2 * dy * pos + sy;
				post = new Point(xa + dax * elm.cspc2, ya + day * elm.cspc2);
				stub = new Point(xa + dax * elm.cspc, ya + day * elm.cspc);
				textloc = new Point(xa, ya);
				if (bubble) {
					bubbleX = xa + dax * 10 * elm.csize;
					bubbleY = ya + day * 10 * elm.csize;
				}
				if (clock) {
					elm.clockPointsX = new int[3];
					elm.clockPointsY = new int[3];
					elm.clockPointsX[0] = xa + dax * elm.cspc - dx * elm.cspc / 2;
					elm.clockPointsY[0] = ya + day * elm.cspc - dy * elm.cspc / 2;
					elm.clockPointsX[1] = xa;
					elm.clockPointsY[1] = ya;
					elm.clockPointsX[2] = xa + dax * elm.cspc + dx * elm.cspc / 2;
					elm.clockPointsY[2] = ya + day * elm.cspc + dy * elm.cspc / 2;
				}
			}
		}

	}
}