namespace Chat_Test
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnl_main = new MetroFramework.Controls.MetroPanel();
            this.treeMem = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pnl_main.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnl_main
            // 
            this.pnl_main.Controls.Add(this.treeMem);
            this.pnl_main.HorizontalScrollbarBarColor = true;
            this.pnl_main.HorizontalScrollbarHighlightOnWheel = false;
            this.pnl_main.HorizontalScrollbarSize = 10;
            this.pnl_main.Location = new System.Drawing.Point(13, 79);
            this.pnl_main.Name = "pnl_main";
            this.pnl_main.Size = new System.Drawing.Size(325, 458);
            this.pnl_main.TabIndex = 0;
            this.pnl_main.VerticalScrollbarBarColor = true;
            this.pnl_main.VerticalScrollbarHighlightOnWheel = false;
            this.pnl_main.VerticalScrollbarSize = 10;
            // 
            // treeMem
            // 
            this.treeMem.Location = new System.Drawing.Point(10, 107);
            this.treeMem.Name = "treeMem";
            this.treeMem.Size = new System.Drawing.Size(303, 353);
            this.treeMem.TabIndex = 2;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 26);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 550);
            this.Controls.Add(this.pnl_main);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.pnl_main.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroPanel pnl_main;
        private System.Windows.Forms.TreeView treeMem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    }
}

