﻿namespace Talifun.Commander.Command.Configuration
{
    partial class CommanderSectionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.OptionDescrLabel = new System.Windows.Forms.Label();
            this.OptDescrSplit = new System.Windows.Forms.SplitContainer();
            this.AppRestartLabel = new System.Windows.Forms.Label();
            this.OptionsPanelPath = new System.Windows.Forms.Label();
            this.OptionsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.OptionPanelContainer = new System.Windows.Forms.Panel();
            this.DescriptionPanel = new System.Windows.Forms.Panel();
            this.ApplyBtn = new System.Windows.Forms.Button();
            this.CommandSectionTreeView = new System.Windows.Forms.TreeView();
            this.OKBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.CatDescrPanel = new System.Windows.Forms.Panel();
            this.CatDescr = new System.Windows.Forms.Label();
            this.CatTreePanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.CatHeaderPanel = new System.Windows.Forms.Panel();
            this.CatHeader = new System.Windows.Forms.Label();
            this.OptionsFormSplit = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.OptDescrSplit)).BeginInit();
            this.OptDescrSplit.Panel1.SuspendLayout();
            this.OptDescrSplit.Panel2.SuspendLayout();
            this.OptDescrSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OptionsSplitContainer)).BeginInit();
            this.OptionsSplitContainer.Panel1.SuspendLayout();
            this.OptionsSplitContainer.Panel2.SuspendLayout();
            this.OptionsSplitContainer.SuspendLayout();
            this.DescriptionPanel.SuspendLayout();
            this.CatDescrPanel.SuspendLayout();
            this.CatTreePanel.SuspendLayout();
            this.CatHeaderPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OptionsFormSplit)).BeginInit();
            this.OptionsFormSplit.Panel1.SuspendLayout();
            this.OptionsFormSplit.Panel2.SuspendLayout();
            this.OptionsFormSplit.SuspendLayout();
            this.SuspendLayout();
            // 
            // OptionDescrLabel
            // 
            this.OptionDescrLabel.AccessibleDescription = "This box shows a short description for each control.";
            this.OptionDescrLabel.AutoEllipsis = true;
            this.OptionDescrLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OptionDescrLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.OptionDescrLabel.Location = new System.Drawing.Point(0, 0);
            this.OptionDescrLabel.Name = "OptionDescrLabel";
            this.OptionDescrLabel.Size = new System.Drawing.Size(473, 62);
            this.OptionDescrLabel.TabIndex = 0;
            this.OptionDescrLabel.Text = "Description.";
            this.OptionDescrLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // OptDescrSplit
            // 
            this.OptDescrSplit.BackColor = System.Drawing.SystemColors.ControlLight;
            this.OptDescrSplit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OptDescrSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OptDescrSplit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.OptDescrSplit.IsSplitterFixed = true;
            this.OptDescrSplit.Location = new System.Drawing.Point(0, 0);
            this.OptDescrSplit.Margin = new System.Windows.Forms.Padding(0);
            this.OptDescrSplit.MaximumSize = new System.Drawing.Size(0, 64);
            this.OptDescrSplit.Name = "OptDescrSplit";
            this.OptDescrSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // OptDescrSplit.Panel1
            // 
            this.OptDescrSplit.Panel1.Controls.Add(this.OptionDescrLabel);
            this.OptDescrSplit.Panel1MinSize = 35;
            // 
            // OptDescrSplit.Panel2
            // 
            this.OptDescrSplit.Panel2.Controls.Add(this.AppRestartLabel);
            this.OptDescrSplit.Panel2Collapsed = true;
            this.OptDescrSplit.Panel2MinSize = 17;
            this.OptDescrSplit.Size = new System.Drawing.Size(475, 64);
            this.OptDescrSplit.SplitterDistance = 35;
            this.OptDescrSplit.SplitterWidth = 1;
            this.OptDescrSplit.TabIndex = 0;
            // 
            // AppRestartLabel
            // 
            this.AppRestartLabel.AccessibleDescription = "This box indicates that the application must restart in order for changes to take" +
    " effect.";
            this.AppRestartLabel.AutoEllipsis = true;
            this.AppRestartLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AppRestartLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AppRestartLabel.ForeColor = System.Drawing.Color.Crimson;
            this.AppRestartLabel.Location = new System.Drawing.Point(0, 0);
            this.AppRestartLabel.Name = "AppRestartLabel";
            this.AppRestartLabel.Size = new System.Drawing.Size(52, 23);
            this.AppRestartLabel.TabIndex = 1;
            this.AppRestartLabel.Text = "Application must restart for changes to take effect.";
            this.AppRestartLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // OptionsPanelPath
            // 
            this.OptionsPanelPath.AccessibleDescription = "This box shows the options path for the current options panel.";
            this.OptionsPanelPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OptionsPanelPath.AutoEllipsis = true;
            this.OptionsPanelPath.BackColor = System.Drawing.SystemColors.ControlLight;
            this.OptionsPanelPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OptionsPanelPath.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OptionsPanelPath.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.OptionsPanelPath.Location = new System.Drawing.Point(3, 3);
            this.OptionsPanelPath.Name = "OptionsPanelPath";
            this.OptionsPanelPath.Padding = new System.Windows.Forms.Padding(10, 6, 0, 6);
            this.OptionsPanelPath.Size = new System.Drawing.Size(467, 32);
            this.OptionsPanelPath.TabIndex = 8;
            this.OptionsPanelPath.Text = "Options » Path";
            this.OptionsPanelPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.OptionsPanelPath.Visible = false;
            // 
            // OptionsSplitContainer
            // 
            this.OptionsSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OptionsSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.OptionsSplitContainer.IsSplitterFixed = true;
            this.OptionsSplitContainer.Location = new System.Drawing.Point(3, 37);
            this.OptionsSplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.OptionsSplitContainer.MinimumSize = new System.Drawing.Size(340, 288);
            this.OptionsSplitContainer.Name = "OptionsSplitContainer";
            this.OptionsSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // OptionsSplitContainer.Panel1
            // 
            this.OptionsSplitContainer.Panel1.Controls.Add(this.OptionPanelContainer);
            this.OptionsSplitContainer.Panel1MinSize = 215;
            // 
            // OptionsSplitContainer.Panel2
            // 
            this.OptionsSplitContainer.Panel2.Controls.Add(this.DescriptionPanel);
            this.OptionsSplitContainer.Panel2MinSize = 64;
            this.OptionsSplitContainer.Size = new System.Drawing.Size(475, 550);
            this.OptionsSplitContainer.SplitterDistance = 442;
            this.OptionsSplitContainer.SplitterWidth = 8;
            this.OptionsSplitContainer.TabIndex = 7;
            // 
            // OptionPanelContainer
            // 
            this.OptionPanelContainer.AutoScroll = true;
            this.OptionPanelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OptionPanelContainer.Location = new System.Drawing.Point(0, 0);
            this.OptionPanelContainer.Margin = new System.Windows.Forms.Padding(0);
            this.OptionPanelContainer.MinimumSize = new System.Drawing.Size(340, 215);
            this.OptionPanelContainer.Name = "OptionPanelContainer";
            this.OptionPanelContainer.Size = new System.Drawing.Size(475, 442);
            this.OptionPanelContainer.TabIndex = 5;
            // 
            // DescriptionPanel
            // 
            this.DescriptionPanel.Controls.Add(this.OptDescrSplit);
            this.DescriptionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DescriptionPanel.Location = new System.Drawing.Point(0, 0);
            this.DescriptionPanel.Name = "DescriptionPanel";
            this.DescriptionPanel.Size = new System.Drawing.Size(475, 100);
            this.DescriptionPanel.TabIndex = 4;
            // 
            // ApplyBtn
            // 
            this.ApplyBtn.AccessibleDescription = "Apply changes.";
            this.ApplyBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ApplyBtn.Enabled = false;
            this.ApplyBtn.Location = new System.Drawing.Point(314, 605);
            this.ApplyBtn.Name = "ApplyBtn";
            this.ApplyBtn.Size = new System.Drawing.Size(75, 23);
            this.ApplyBtn.TabIndex = 6;
            this.ApplyBtn.Text = "A&pply";
            this.ApplyBtn.UseVisualStyleBackColor = true;
            // 
            // CommandSectionTreeView
            // 
            this.CommandSectionTreeView.AccessibleDescription = "Options Categories.";
            this.CommandSectionTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CommandSectionTreeView.Location = new System.Drawing.Point(0, 0);
            this.CommandSectionTreeView.Name = "CommandSectionTreeView";
            this.CommandSectionTreeView.ShowLines = false;
            this.CommandSectionTreeView.Size = new System.Drawing.Size(244, 558);
            this.CommandSectionTreeView.TabIndex = 0;
            this.CommandSectionTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.CommandSectionTreeView_AfterSelect);
            this.CommandSectionTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CommandSectionTreeView_MouseDown);
            // 
            // OKBtn
            // 
            this.OKBtn.AccessibleDescription = "Apply changes and close the options form.";
            this.OKBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKBtn.Location = new System.Drawing.Point(233, 605);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(75, 23);
            this.OKBtn.TabIndex = 2;
            this.OKBtn.Text = "&OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            // 
            // CancelBtn
            // 
            this.CancelBtn.AccessibleDescription = "Cancel changes and close options form.";
            this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(395, 605);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 1;
            this.CancelBtn.Text = "&Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // CatDescrPanel
            // 
            this.CatDescrPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CatDescrPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CatDescrPanel.Controls.Add(this.CatDescr);
            this.CatDescrPanel.Location = new System.Drawing.Point(4, 597);
            this.CatDescrPanel.Margin = new System.Windows.Forms.Padding(0);
            this.CatDescrPanel.Name = "CatDescrPanel";
            this.CatDescrPanel.Size = new System.Drawing.Size(243, 37);
            this.CatDescrPanel.TabIndex = 2;
            this.CatDescrPanel.Visible = false;
            // 
            // CatDescr
            // 
            this.CatDescr.AccessibleDescription = "This box shows a description for each Category in the tree.";
            this.CatDescr.AutoEllipsis = true;
            this.CatDescr.BackColor = System.Drawing.SystemColors.ControlLight;
            this.CatDescr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CatDescr.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CatDescr.Location = new System.Drawing.Point(0, 0);
            this.CatDescr.Name = "CatDescr";
            this.CatDescr.Size = new System.Drawing.Size(241, 35);
            this.CatDescr.TabIndex = 0;
            this.CatDescr.Text = "Description.";
            this.CatDescr.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CatTreePanel
            // 
            this.CatTreePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CatTreePanel.Controls.Add(this.CommandSectionTreeView);
            this.CatTreePanel.Location = new System.Drawing.Point(3, 37);
            this.CatTreePanel.Margin = new System.Windows.Forms.Padding(0);
            this.CatTreePanel.Name = "CatTreePanel";
            this.CatTreePanel.Size = new System.Drawing.Size(244, 558);
            this.CatTreePanel.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(3, 597);
            this.label2.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(467, 2);
            this.label2.TabIndex = 0;
            // 
            // CatHeaderPanel
            // 
            this.CatHeaderPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CatHeaderPanel.Controls.Add(this.CatHeader);
            this.CatHeaderPanel.Location = new System.Drawing.Point(3, 3);
            this.CatHeaderPanel.Margin = new System.Windows.Forms.Padding(0);
            this.CatHeaderPanel.Name = "CatHeaderPanel";
            this.CatHeaderPanel.Size = new System.Drawing.Size(244, 32);
            this.CatHeaderPanel.TabIndex = 0;
            this.CatHeaderPanel.Visible = false;
            // 
            // CatHeader
            // 
            this.CatHeader.AccessibleDescription = "Categories tree Header.";
            this.CatHeader.AutoEllipsis = true;
            this.CatHeader.BackColor = System.Drawing.SystemColors.ControlLight;
            this.CatHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CatHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CatHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CatHeader.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CatHeader.Location = new System.Drawing.Point(0, 0);
            this.CatHeader.Name = "CatHeader";
            this.CatHeader.Size = new System.Drawing.Size(244, 32);
            this.CatHeader.TabIndex = 0;
            this.CatHeader.Text = "Categories:";
            this.CatHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // OptionsFormSplit
            // 
            this.OptionsFormSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OptionsFormSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.OptionsFormSplit.IsSplitterFixed = true;
            this.OptionsFormSplit.Location = new System.Drawing.Point(0, 0);
            this.OptionsFormSplit.Margin = new System.Windows.Forms.Padding(0);
            this.OptionsFormSplit.MinimumSize = new System.Drawing.Size(480, 375);
            this.OptionsFormSplit.Name = "OptionsFormSplit";
            // 
            // OptionsFormSplit.Panel1
            // 
            this.OptionsFormSplit.Panel1.Controls.Add(this.CatDescrPanel);
            this.OptionsFormSplit.Panel1.Controls.Add(this.CatTreePanel);
            this.OptionsFormSplit.Panel1.Controls.Add(this.CatHeaderPanel);
            this.OptionsFormSplit.Panel1MinSize = 250;
            // 
            // OptionsFormSplit.Panel2
            // 
            this.OptionsFormSplit.Panel2.Controls.Add(this.OptionsSplitContainer);
            this.OptionsFormSplit.Panel2.Controls.Add(this.OptionsPanelPath);
            this.OptionsFormSplit.Panel2.Controls.Add(this.OKBtn);
            this.OptionsFormSplit.Panel2.Controls.Add(this.ApplyBtn);
            this.OptionsFormSplit.Panel2.Controls.Add(this.CancelBtn);
            this.OptionsFormSplit.Panel2.Controls.Add(this.label2);
            this.OptionsFormSplit.Panel2MinSize = 450;
            this.OptionsFormSplit.Size = new System.Drawing.Size(747, 637);
            this.OptionsFormSplit.SplitterDistance = 250;
            this.OptionsFormSplit.SplitterWidth = 5;
            this.OptionsFormSplit.TabIndex = 1;
            // 
            // CommanderSectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 637);
            this.Controls.Add(this.OptionsFormSplit);
            this.Name = "CommanderSectionForm";
            this.Text = "CommanderSectionForm";
            this.OptDescrSplit.Panel1.ResumeLayout(false);
            this.OptDescrSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.OptDescrSplit)).EndInit();
            this.OptDescrSplit.ResumeLayout(false);
            this.OptionsSplitContainer.Panel1.ResumeLayout(false);
            this.OptionsSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.OptionsSplitContainer)).EndInit();
            this.OptionsSplitContainer.ResumeLayout(false);
            this.DescriptionPanel.ResumeLayout(false);
            this.CatDescrPanel.ResumeLayout(false);
            this.CatTreePanel.ResumeLayout(false);
            this.CatHeaderPanel.ResumeLayout(false);
            this.OptionsFormSplit.Panel1.ResumeLayout(false);
            this.OptionsFormSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.OptionsFormSplit)).EndInit();
            this.OptionsFormSplit.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label OptionDescrLabel;
        private System.Windows.Forms.SplitContainer OptDescrSplit;
        private System.Windows.Forms.Label AppRestartLabel;
        private System.Windows.Forms.Label OptionsPanelPath;
        private System.Windows.Forms.SplitContainer OptionsSplitContainer;
        private System.Windows.Forms.Panel OptionPanelContainer;
        private System.Windows.Forms.Panel DescriptionPanel;
        private System.Windows.Forms.Button ApplyBtn;
        private System.Windows.Forms.TreeView CommandSectionTreeView;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Panel CatDescrPanel;
        private System.Windows.Forms.Label CatDescr;
        private System.Windows.Forms.Panel CatTreePanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel CatHeaderPanel;
        private System.Windows.Forms.Label CatHeader;
        private System.Windows.Forms.SplitContainer OptionsFormSplit;

    }
}