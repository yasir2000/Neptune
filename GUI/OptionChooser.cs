//
// Copyright (c) 2009 Froduald Kabanza and the Université de Sherbrooke.
// Use of this software is permitted for non-commercial research purposes, and
// it may be copied or applied only for that use. All copies must include this
// copyright message.
// 
// This is a research prototype and it has not gone through intensive tests and
// is delivered as is. It may still contain bugs. Froduald Kabanza and the
// Université de Sherbrooke disclaim any responsibility for damage that may be
// caused by using it.
// 
// Implementation: Daniel Castonguay
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using TLPlan;
using GUI.Properties;

namespace GUI
{
    /// <summary>
    /// Represents the main options GUI window of the TLPlan planner.
    /// This window allows for choosing different otions and starting the planning.
    /// </summary>
    public partial class OptionChooser : Form
    {
        #region Private Fields

        /// <summary>
        /// The options chosen by the user.
        /// </summary>
        private GUITLPlanOptions m_options;
        /// <summary>
        /// The number of problems that have been started so far.
        /// </summary>
        private int m_problemCount;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new option chooser window.
        /// </summary>
        public OptionChooser()
        {
            InitializeComponent();
            m_options = new GUITLPlanOptions();
            m_problemCount = 0;
        }

        #endregion

        #region Event Handlers

        private void OptionChooser_Load(object sender, EventArgs e)
        {
            m_options.DomainFile = Settings.Default.DomainFile ?? string.Empty;
            m_options.ProblemFile = Settings.Default.ProblemFile ?? string.Empty;

            pgrdOptions.SelectedObject = m_options;
        }

        private void OptionChooser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }

            SaveUserSettings();
        }

        private void tsbtnStartOne_Click(object sender, EventArgs e)
        {
            // TODO: IMPORTANT: Check the actual parsability of the domain! D:
            try
            {
                ViewerForm viewerForm = new ViewerForm(m_options.GetSelectedGUITLPlanOptions().First());
                viewerForm.MdiParent = this.MdiParent;
                // TODO: Add an appropriate header?
                viewerForm.Text = "Neptune TLPLAN Output: (Problem " + this.m_problemCount++ + ")";
                viewerForm.Show();
                //viewerForm.StepIn();
                viewerForm.Start();

                SaveUserSettings();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(string.Format("An exception occured while starting the planning.\n\n{0}", ex.Message),
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbtnStartAll_Click(object sender, EventArgs e)
        {
            int count = m_options.Count;
            bool solve = true;

            if (count > 5)
            {
                if (MessageBox.Show(string.Format("You are trying to solve {0} instances of the problem using different options. The application may become unresponsive for several seconds or several minutes.\n\nDo you wish to continue and solve the problem using {0} options?", count),
                                    string.Format("Solving a problem using {0} options", count),
                                    MessageBoxButtons.YesNoCancel,
                                    MessageBoxIcon.Warning,
                                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                {
                    solve = false;
                }
            }

            if (solve)
            {
                try
                {
                    foreach (GUITLPlanOptions options in m_options.GetSelectedGUITLPlanOptions())
                    {
                        ViewerForm viewerForm = new ViewerForm(options);
                        viewerForm.MdiParent = this.MdiParent;
                        // TODO: Add an appropriate header?
                        viewerForm.Text = "Neptune TLPLAN Output: (Problem " + this.m_problemCount++ + ")";
                        viewerForm.Show();
                        viewerForm.Start();
                    }

                    SaveUserSettings();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(string.Format("An exception occured while starting the planning.\n\n{0}", ex.Message),
                                  "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Saves the user's choices in the settings, so that they can be loaded the next time the application is run.
        /// </summary>
        private void SaveUserSettings()
        {
            Settings.Default.DomainFile = m_options.DomainFile;
            Settings.Default.ProblemFile = m_options.ProblemFile;

            Settings.Default.Save();
        }

        #endregion
    }
}
